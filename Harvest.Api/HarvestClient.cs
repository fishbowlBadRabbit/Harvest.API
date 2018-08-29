﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Harvest.Api
{
    public class HarvestClient : IHarvestClient, IDisposable
    {
        #region Constants
        private const string tokenType = "Bearer";
        private const string harvestIdUrl = "https://id.getharvest.com";
        private const string harvestApiUrl = "https://api.harvestapp.com/v2";
        #endregion

        #region Members
        private HttpClient _httpClient;
        private RequestBuilder _requestBuilder = new RequestBuilder();
        #endregion

        #region Properties
        public long? DefaultAccountId { get; set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime ExpireAt { get; private set; }
        public string AuthState { get; private set; }

        public string UserAgent { get; private set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri RedirectUri { get; set; }
        #endregion

        #region Events
        public event EventHandler TokenRefreshed;
        #endregion

        #region Constructor
        public HarvestClient(string userAgent, HttpClientHandler httpClientHandler = null)
        {
            if (string.IsNullOrEmpty(userAgent))
                throw new ArgumentNullException(nameof(userAgent));

            this.UserAgent = userAgent;

            _httpClient = new HttpClient(httpClientHandler ?? new HttpClientHandler());
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(RequestBuilder.JsonMimeType));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", this.UserAgent);
        }
        #endregion

        #region Methods
        public static HarvestClient FromAccessToken(string userAgent, string accessToken, string refreshToken = null, long expiresIn = 0, HttpClientHandler httpClientHandler = null)
        {
            var client = new HarvestClient(userAgent, httpClientHandler);

            client.Authorize(accessToken, refreshToken, expiresIn);

            return client;
        }
        #endregion

        #region Auth methods
        public Uri BuildAuthorizationUrl(string state = null, string scope = null, bool codeType = true)
        {
            if (string.IsNullOrEmpty(ClientId))
                throw new InvalidOperationException("ClientId is empty or null");

            if (RedirectUri == null)
                throw new InvalidOperationException("RedirectUri is null");

            AuthState = state ?? Utilities.GenerateState();

            var query = new Dictionary<string, string>
            {
                { "client_id", this.ClientId },
                { "redirect_uri", this.RedirectUri.ToString() },
                { "state", AuthState },
                { "scope", scope },
                { "response_type", codeType ? "code" : "token" }
            };

            return RequestBuilder.BuildUri($"{harvestIdUrl}/oauth2/authorize", query);
        }

        public async Task<AuthResponse> AuthorizeAsync(Uri callbackUri, string state = null, bool defaultAccountId = true)
        {
            var query = Utilities.ParseQueryString(callbackUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped));

            if (!query.TryGetValue("state", out var urlState) || urlState != (state ?? AuthState))
                throw new InvalidOperationException("OAuth states doesn't match");

            AuthResponse result = null;

            if (query.TryGetValue("token_type", out var tokenType) &&
                query.TryGetValue("access_token", out var accessToken))
            {
                query.TryGetValue("expires_in", out var expiresIn);

                result = new AuthResponse { AccessToken = accessToken, TokenType = tokenType, ExpiresIn = long.Parse(expiresIn) };
            }
            else if (query.TryGetValue("code", out var code))
            {
                result = await new RequestBuilder()
                    .Begin(HttpMethod.Post, $"{harvestIdUrl}/api/v1/oauth2/token")
                    .Form("code", code)
                    .Form("client_id", this.ClientId)
                    .Form("client_secret", this.ClientSecret)
                    .Form("grant_type", "authorization_code")
                    .SendAsync<AuthResponse>(_httpClient);
            }

            if (result == null)
                throw new ArgumentException(nameof(callbackUri));

            Authorize(result.AccessToken, result.RefreshToken, result.ExpiresIn);

            query.TryGetValue("scope", out var scope);
            result.Scope = scope;

            if (defaultAccountId)
                this.DefaultAccountId = Utilities.FirstHarvestAccountId(scope);

            return result;
        }

        public async Task<AuthResponse> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(this.RefreshToken))
                throw new InvalidOperationException("Refresh token is empty");

            var result = await new RequestBuilder()
                .Begin(HttpMethod.Post, $"{harvestIdUrl}/api/v1/oauth2/token")
                .Form("client_id", this.ClientId)
                .Form("client_secret", this.ClientSecret)
                .Form("grant_type", "refresh_token")
                .Form("refresh_token", this.RefreshToken)
                .SendAsync<AuthResponse>(_httpClient);

            Authorize(result.AccessToken, result.RefreshToken, result.ExpiresIn);

            return result;
        }

        public void Authorize(string accessToken, string refreshToken = null, long expiresIn = 0)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException(nameof(accessToken));

            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.ExpireAt = DateTime.Now.AddSeconds(expiresIn);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            this.TokenRefreshed?.Invoke(this, EventArgs.Empty);
        }

        public bool IsRedirectUri(Uri uri)
        {
            if (RedirectUri == null)
                throw new InvalidOperationException("RedirectUri is null");

            return uri.GetLeftPart(UriPartial.Path) == this.RedirectUri.GetLeftPart(UriPartial.Path);
        }
        #endregion

        #region API methods
        public async Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await _requestBuilder
                .Begin($"{harvestIdUrl}/api/v1/accounts")
                .SendAsync<AccountsResponse>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntriesResponse> GetTimeEntriesAsync(long? userId = null, long? clientId = null, long? projectId = null, bool? isBilled = null,
            DateTime? updatedSince = null, DateTime? fromDate = null, DateTime? toDate = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries", accountId)
                .Query("user_id", userId)
                .Query("client_id", clientId)
                .Query("project_id", projectId)
                .Query("is_billed", isBilled)
                .Query("updated_since", updatedSince)
                .Query("from", fromDate)
                .Query("to", toDate)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<TimeEntriesResponse>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> GetTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries/{entryId}", accountId)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> RestartTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries/{entryId}/restart", accountId, RequestBuilder.PatchMethod)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> StopTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries/{entryId}/stop", accountId, RequestBuilder.PatchMethod)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async ThreadingTask DeleteTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            await SimpleRequestBuilder($"{harvestApiUrl}/time_entries/{entryId}", accountId, HttpMethod.Delete)
                .SendAsync(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> CreateTimeEntryAsync(long projectId, long taskId, DateTime spentDate,
            TimeSpan? startedTime = null, TimeSpan? endedTime = null, decimal? hours = null, string notes = null, ExternalReference externalReference = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries", accountId, HttpMethod.Post)
                .Form("project_id", projectId)
                .Form("task_id", taskId)
                .Form("spent_date", spentDate)
                .Form("started_time", startedTime)
                .Form("ended_time", endedTime)
                .Form("hours", hours)
                .Form("notes", notes)
                .Form("external_reference", externalReference)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> UpdateTimeEntryAsync(long entryId,
            long? projectId = null, long? taskId = null, DateTime? spentDate = null, TimeSpan? startedTime = null, TimeSpan? endedTime = null,
            decimal? hours = null, string notes = null, ExternalReference externalReference = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/time_entries/{entryId}", accountId, RequestBuilder.PatchMethod)
                .Form("project_id", projectId)
                .Form("task_id", taskId)
                .Form("spent_date", spentDate)
                .Form("started_time", startedTime)
                .Form("ended_time", endedTime)
                .Form("hours", hours)
                .Form("notes", notes)
                .Form("external_reference", externalReference)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async Task<ProjectAssignmentsResponse> GetProjectAssignmentsAsync(long? userId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            var userIdOrMe = userId.HasValue ? userId.ToString() : "me";

            return await SimpleRequestBuilder($"{harvestApiUrl}/users/{userIdOrMe}/project_assignments", accountId)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<ProjectAssignmentsResponse>(_httpClient, cancellationToken);
        }

        public async Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/projects", accountId)
                .Query("client_id", clientId)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<ProjectsResponse>(_httpClient, cancellationToken);
        }

        public async Task<TasksResponse> GetTasksAsync(DateTime? updatedSince = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/tasks", accountId)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<TasksResponse>(_httpClient, cancellationToken);
        }

        public async Task<UserDetails> GetMe(long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/users/me", accountId)
                .SendAsync<UserDetails>(_httpClient, cancellationToken);
        }

        public async Task<UserDetails> GetUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/users/{userId}", accountId)
                .SendAsync<UserDetails>(_httpClient, cancellationToken);
        }

        public async Task<UsersResponse> GetUsers(bool? isActive = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/users", accountId)
                .Query("is_active", isActive)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<UsersResponse>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> CreateUser(string firstName, string lastName, string email,
            string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
            bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
            bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
            bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
            decimal? costRate = null, string[] roles = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/users", accountId, HttpMethod.Post)
                .Form("first_name", firstName)
                .Form("last_name", lastName)
                .Form("email", email)
                .Form("telephone", telephone)
                .Form("timezone", telephone)
                .Form("has_access_to_all_future_projects", hasAccessToAllFutureProjects)
                .Form("is_contractor", isContractor)
                .Form("is_admin", isAdmin)
                .Form("is_project_manager", isProjectManager)
                .Form("can_see_rates", canSeeRates)
                .Form("can_create_projects", canCreateProjects)
                .Form("can_create_invoices", canCreateInvoices)
                .Form("is_active", isActive)
                .Form("weekly_capacity", weeklyCapacity)
                .Form("default_hourly_rate", defaultHourlyRate)
                .Form("cost_rate", costRate)
                //TODO .Form("roles", roles)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async Task<TimeEntry> UpdateUser(int userId, string firstName = null, string lastName = null, string email = null,
            string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
            bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
            bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
            bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
            decimal? costRate = null, string[] roles = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/users/{userId}", accountId, RequestBuilder.PatchMethod)
                .Form("first_name", firstName)
                .Form("last_name", lastName)
                .Form("email", email)
                .Form("telephone", telephone)
                .Form("timezone", telephone)
                .Form("has_access_to_all_future_projects", hasAccessToAllFutureProjects)
                .Form("is_contractor", isContractor)
                .Form("is_admin", isAdmin)
                .Form("is_project_manager", isProjectManager)
                .Form("can_see_rates", canSeeRates)
                .Form("can_create_projects", canCreateProjects)
                .Form("can_create_invoices", canCreateInvoices)
                .Form("is_active", isActive)
                .Form("weekly_capacity", weeklyCapacity)
                .Form("default_hourly_rate", defaultHourlyRate)
                .Form("cost_rate", costRate)
                //TODO .Form("roles", roles)
                .SendAsync<TimeEntry>(_httpClient, cancellationToken);
        }

        public async ThreadingTask DeleteUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            await SimpleRequestBuilder($"{harvestApiUrl}/users/{userId}", accountId, HttpMethod.Delete)
                .SendAsync(_httpClient, cancellationToken);
        }

        #region Clients

        public async Task<Client> CreateClient(string name, string details, bool active, string currency, long? highriseId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();
            return await SimpleRequestBuilder($"{harvestApiUrl}/clients", accountId, HttpMethod.Post)
                .Form("name", name)
                .Form("is_active", active)
                .Form("currency", currency)
                .Form("details", details)
                .SendAsync<Client>(_httpClient, cancellationToken);
        }
        //public async Task<Client> CreateClient(Client client, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    await RefreshTokenIsNeeded();
        //    return await SimpleRequestBuilder($"{harvestApiUrl}/clients", accountId, HttpMethod.Post)
        //        .Form("name", client.Name)
        //        .Form("is_active", client.IsActive)
        //        .Form("currency", client.Currency.ToString())
        //        .SendAsync<Client>(_httpClient, cancellationToken);
        //}

        public async Task<Client> GetClient(long clientId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/clients/{clientId}", clientId)
               .SendAsync<Client>(_httpClient, cancellationToken);
        }

        public async Task<ClientsResponse> GetClients(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/clients/", accountId)
               .Query("page", page)
               .Query("per_page", perPage)
               .Query("updated_since", updatedSince)
               .SendAsync<ClientsResponse>(_httpClient, cancellationToken);
        }

        #endregion

        #region ClientContacts

        public async Task<ClientContactsResponse> GetContacts(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/contacts/", accountId)
               .Query("page", page)
               .Query("per_page", perPage)
               .Query("updated_since", updatedSince)
               .SendAsync<ClientContactsResponse>(_httpClient, cancellationToken);
        }

        #endregion

        #region ExpenseCategory

        public async Task<ExpenseCategoryResponse> GetExpenseCategories(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/expense_categories/", accountId)
                .Query("page", page)
                .Query("per_page", perPage)
                .Query("updated_since", updatedSince)
                .SendAsync<ExpenseCategoryResponse>(_httpClient, cancellationToken);
        }

        public async Task<ExpenseCategory> GetExpenseCategory(long categoryID, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/expense_categories/{categoryID}", accountId)
                 .SendAsync<ExpenseCategory>(_httpClient, cancellationToken);
        }

        #endregion

        #region Invoice

        public async Task<Invoice> CreateInvoice(long? clientId, string currency, DateTime? issuedAt, DateTime? dueAt, string number, string subject, string purchaseOrder, string clientKey, string notes, decimal? tax, decimal? tax2, decimal? taxAmount, decimal? tax2Amount, string kind, string lineItems, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/invoices", accountId, HttpMethod.Post)
                .Form("client_id", clientId)
                .Form("number", number)
                .Form("purchase_order", purchaseOrder)
                .Form("tax", tax)
                .Form("tax2", tax2)
                .Form("subject", subject)
                .Form("notes", notes)
                .Form("currency", currency)
                .Form("issue_date", issuedAt)
                .Form("due_date", dueAt)
                .Form("line_items", String.Join(",", lineItems))
                .SendAsync<Invoice>(_httpClient, cancellationToken);
        }

        public async Task<bool> DeleteInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            await SimpleRequestBuilder($"{harvestApiUrl}/invoices/{invoiceId}", accountId, HttpMethod.Delete)
                .SendAsync(_httpClient, cancellationToken);

            return true;
        }

        public async Task<Invoice> GetInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/invoices/{invoiceId}", accountId)
                .SendAsync<Invoice>(_httpClient, cancellationToken);
        }

        public async Task<InvoiceResponse> GetInvoices(DateTime? updatedSince, int? page = 1, int? perPage = 100, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/invoices", accountId)
                .Query("page", page)
                .Query("per_page", perPage)
                .Query("updated_since", updatedSince)
                .SendAsync<InvoiceResponse>(_httpClient, cancellationToken);
        }
        #endregion

        #region Project

        public async Task<Project> CreateProject(string name, long? clientId, bool? active, string billBy, string code, string notes, string budgetBy, decimal? budget, bool? billable, decimal? hourlyRate, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? startsOn, DateTime? endsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/projects", accountId, HttpMethod.Post)
                .Form("client_id", clientId)
                .Form("name", name)
                .Form("code", code)
                .Form("is_active", active)
                .Form("is_billable", billable)
                .Form("bill_by", billBy)
                .Form("hourly_rate", hourlyRate)
                .Form("budget", budget)
                .Form("budget_by", budgetBy)
                .Form("notify_when_over_budget", notifyWhenOverBudget)
                .Form("over_budget_notification_percentage", overBudgetNotificationPercentage)
                .Form("show_budget_to_all", showBudgetToAll)
                .Form("cost_budget", costBudget)
                .Form("cost_budget_include_expenses", costBudgetIncludeExpenses)
                .Form("notes", notes)
                .Form("starts_on", startsOn)
                .Form("ends_on", endsOn)
                .SendAsync<Project>(_httpClient, cancellationToken);
        }

        //public async Task<Project> CreateProject(Project project, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    await RefreshTokenIsNeeded();

        //    return await SimpleRequestBuilder($"{harvestApiUrl}/projects", accountId, HttpMethod.Post)
        //        .Form("client_id", project.Client.Id)
        //        .Form("name", project.Name)
        //        .Form("code", project.Code)
        //        .Form("is_active", project.IsActive)
        //        .Form("is_billable", project.IsBillable)
        //        .Form("bill_by", project.BillBy)
        //        .Form("hourly_rate", project.HourlyRate)
        //        .Form("budget", project.Budget)
        //        .Form("budget_by", project.BudgetBy)
        //        .Form("notify_when_over_budget", project.NotifyWhenOverBudget)
        //        .Form("over_budget_notification_percentage", project.OverBudgetNotificationPercentage)
        //        .Form("show_budget_to_all", project.ShowBudgetToAll)
        //        .Form("cost_budget", project.CostBudget)
        //        .Form("cost_budget_include_expenses", project.CostBudgetIncludeExpenses)
        //        .Form("notes", project.Notes)
        //        .Form("starts_on", project.StartsOn)
        //        .Form("ends_on", project.EndsOn)
        //        .SendAsync<Project>(_httpClient, cancellationToken);
        //}

        public async Task<Project> GetProject(long projectId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/projects/{projectId}", accountId)
             .SendAsync<Project>(_httpClient, cancellationToken);
        }

        public async Task<ProjectsResponse> GetProjects(long? clientId, DateTime? updatedSince, long? accountId = null, int? page = 1, int? perPage = 100, bool? isActive = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            var response = await SimpleRequestBuilder($"{harvestApiUrl}/projects/", accountId)
             .Query("client_id", clientId)
             .Query("is_active", isActive)
             .Query("page", page)
             .Query("per_page", perPage)
             .Query("updated_since", updatedSince)
             .SendAsync<ProjectsResponse>(_httpClient, cancellationToken);

            return response;
        }

        public async Task<Project> UpdateProject(long projectId, bool? active, bool? billable, string billBy, decimal? budget, string budgetBy, long? clientId, string code, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? endsOn, decimal? estimate, string estimateBy, decimal? hourlyRate, string name, string notes, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, DateTime? startsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/contacts/", accountId, new HttpMethod("PATCH"))
            .Form("client_id", clientId)
            .Form("name", name)
            .Form("code", code)
            .Form("is_active", active)
            .Form("is_billable", billable)
            .Form("bill_by", billBy)
            .Form("hourly_rate", hourlyRate)
            .Form("budget", budget)
            .Form("budget_by", budgetBy)
            .Form("notify_when_over_budget", notifyWhenOverBudget)
            .Form("over_budget_notification_percentage", overBudgetNotificationPercentage)
            .Form("show_budget_to_all", showBudgetToAll)
            .Form("cost_budget", costBudget)
            .Form("cost_budget_include_expenses", costBudgetIncludeExpenses)
            .Form("notes", notes)
            .Form("starts_on", startsOn)
            .Form("ends_on", endsOn)
           .SendAsync<Project>(_httpClient, cancellationToken);
        }

        //public async Task<Project> UpdateProject(Project project, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    await RefreshTokenIsNeeded();

        //    return await UpdateProject(

        //        project.Id,
        //        project.IsActive,
        //        project.IsBillable,
        //        project.BillBy,
        //        project.Budget,
        //        project.BudgetBy,
        //        project.Client.Id,
        //        project.Code,
        //        project.CostBudget,
        //        project.CostBudgetIncludeExpenses,
        //        project.EndsOn,
        //        project.Estimate,
        //        project.EstimateBy,
        //        project.HourlyRate,
        //        project.Name,
        //        project.Notes,
        //        project.NotifyWhenOverBudget,
        //        project.OverBudgetNotificationPercentage,
        //        project.ShowBudgetToAll,
        //        project.StartsOn,

        //        accountId,
        //        cancellationToken

        //        );
        //}
        #endregion

        #region Expenses
        public async Task<ExpensesResponse> GetExpenses(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, long? projectId = null, long? clientId = null, long? userId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/expenses/", accountId)
                .Query("project_id", projectId)
                .Query("client_id", clientId)
                .Query("user_id", userId)
                .Query("from", from)
                .Query("to", to)
                .Query("page", page)
                .Query("per_page", perPage)
                .Query("updated_since", updatedSince)
                .SendAsync<ExpensesResponse>(_httpClient, cancellationToken);
        }
        #endregion

        #region Payment
        public async Task<PaymentsResponse> GetPayments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? invoiceId = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/payments/", accountId)
                .Query("invoice_id", invoiceId)
                .Query("page", page)
                .Query("per_page", perPage)
                .Query("updated_since", updatedSince)
                .SendAsync<PaymentsResponse>(_httpClient, cancellationToken);
        }
        #endregion

        #region task
        public async Task<Task> GetTask(long taskId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/tasks/{taskId}", accountId)
                .SendAsync<Task>(_httpClient, cancellationToken);
        }
        #endregion

        #region taskAssignment
        public async Task<TaskAssignment> GetTaskAssignment(long projectId, long assignmentId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/projects/{projectId}/task_assignments/{assignmentId}", accountId)
                .SendAsync<TaskAssignment>(_httpClient, cancellationToken);
        }

        public async Task<TaskAssignmentsResponse> GetTaskAssignments(long? projectId, DateTime? updatedSince, int? page, int? perPage, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SimpleRequestBuilder($"{harvestApiUrl}/projects/{projectId}/task_assignments", accountId)
                .Query("page", page)
                .Query("per_page", perPage)
                .Query("updated_since", updatedSince)
                .Query("is_active", isActive)
                .SendAsync<TaskAssignmentsResponse>(_httpClient, cancellationToken);
        }
        #endregion

        #region UserAssignment
        public async Task<UserAssignmentsResponse> GetUserAssignments(DateTime? updatedSince = null, long? projectId = null, int? page = 1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RefreshTokenIsNeeded();

            return await SimpleRequestBuilder($"{harvestApiUrl}/projects/{projectId}/user_assignments", accountId)
               .Query("page", page)
               .Query("per_page", perPage)
               .Query("updated_since", updatedSince)
               .Query("is_active", isActive)
               .SendAsync<UserAssignmentsResponse>(_httpClient, cancellationToken);
        }
        #endregion

        #endregion

        #region Implementation
        private async System.Threading.Tasks.Task RefreshTokenIsNeeded()
        {
            if (this.ExpireAt <= DateTime.Now && !string.IsNullOrEmpty(this.RefreshToken))
                await RefreshTokenAsync();
        }

        private RequestBuilder SimpleRequestBuilder(string url, long? accountId, HttpMethod httpMethod = null)
        {
            if (accountId == null && this.DefaultAccountId == null)
                throw new HarvestException("accountId or DefaultAccountId should be specified");

            return _requestBuilder.Begin(httpMethod ?? HttpMethod.Get, url)
                .AccountId(accountId ?? this.DefaultAccountId);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
        #endregion
    }
}
