using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Harvest.Api
{
    public interface IHarvestClient
    {
        Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntriesResponse> GetTimeEntriesAsync(long? userId = null, long? clientId = null, long? projectId = null, bool? isBilled = null,
          DateTime? updatedSince = null, DateTime? fromDate = null, DateTime? toDate = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> GetTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> RestartTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> StopTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        ThreadingTask DeleteTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> CreateTimeEntryAsync(long projectId, long taskId, DateTime spentDate,
          TimeSpan? startedTime = null, TimeSpan? endedTime = null, decimal? hours = null, string notes = null, ExternalReference externalReference = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> UpdateTimeEntryAsync(long entryId,
          long? projectId = null, long? taskId = null, DateTime? spentDate = null, TimeSpan? startedTime = null, TimeSpan? endedTime = null,
          decimal? hours = null, string notes = null, ExternalReference externalReference = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ProjectAssignmentsResponse> GetProjectAssignmentsAsync(long? userId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<TasksResponse> GetTasksAsync(DateTime? updatedSince = null, int? page = null, int? perPage = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<UserDetails> GetMe(long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<UserDetails> GetUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<UsersResponse> GetUsers(bool? isActive = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> CreateUser(string firstName, string lastName, string email,
          string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
          bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
          bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
          bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
          decimal? costRate = null, string[] roles = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TimeEntry> UpdateUser(int userId, string firstName = null, string lastName = null, string email = null,
          string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
          bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
          bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
          bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
          decimal? costRate = null, string[] roles = null,
          long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        ThreadingTask DeleteUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        #region Clients

        Task<Client> CreateClientAsync(string name, bool? isActive = null, string address = null,
            string currency = null, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Client> GetClientAsync(long clientId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ClientsResponse> GetClientsAsync(bool? isActive = null, DateTime? updatedSince=null, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region ClientContacts

        Task<ClientContactsResponse> GetContacts(DateTime? updatedSince=null, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region ExpenseCategory

        Task<ExpenseCategoriesResponse> GetExpenseCategories(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ExpenseCategory> GetExpenseCategory(long categoryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Invoice

        Task<Invoice> CreateInvoice(long? clientId, string currency, DateTime? issuedAt, DateTime? dueAt, string number, string subject, string purchaseOrder, string clientKey, string notes, decimal? tax, decimal? tax2, decimal? taxAmount, decimal? tax2Amount, string kind, List<LineItem> lineItems, string paymentTerms, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Invoice> GetInvoiceAsync(string invoiceId, long? clientId = null, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<InvoicesResponse> GetInvoicesAsync(long? clientId = null, long? projectId = null,
            InvoiceState? state = null, DateTime? from = null, DateTime? to = null,
            DateTime? updatedSince = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Project

        Task<Project> CreateProjectAsync(long clientId, string name, bool isBillable, string billBy = "none",
            string code = null, bool? isFixedFee = null, decimal? hourlyRate = null, decimal? budget = null,
            string budgetBy = "none",
            bool? budgetIsMonthly = null, bool? notifyWhenOverBudget = null,
            bool? overBudgetNotificationPercentage = null,
            bool? showBudgetToAll = null, decimal? costBudget = null, bool? costBudgetIncludeExpenses = null,
            decimal? fee = null, string notes = null, DateTime? startsOn = null, DateTime? endsOn = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Project> GetProjectAsync(long projectId, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, int? page = null,
            int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Project> UpdateProject(long projectId, bool? active, bool? billable, string billBy, decimal? budget, string budgetBy, long? clientId, string code, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? endsOn, decimal? estimate, string estimateBy, decimal? hourlyRate, string name, string notes, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, DateTime? startsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Expenses

        Task<ExpensesResponse> GetExpenses(long? userId = null, long? clientId = null, long? projectId = null,
            bool? isBilled = null,
            DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Payment
        Task<PaymentsResponse> GetPayments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? invoiceId = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region task
        Task<Task> GetTaskAsync(long taskId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region taskAssignment

        Task<TaskAssignment> GetTaskAssignmentAsync(long? projectId = null, bool? isActive = null,DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TaskAssignmentsResponse> GetTaskAssignmentsAsync(DateTime? updatedSince = null, int? page =1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region UserAssignment
        Task<UserAssignmentsResponse> GetUserAssignments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion
    }
}
