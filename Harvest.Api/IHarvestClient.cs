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

        Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
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

        Task<Client> CreateClient(string name, string details, bool active, string currency, long? highriseId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Client> GetClient(long clientId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ClientsResponse> GetClients(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region ClientContacts

        Task<ClientContactsResponse> GetContacts(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region ExpenseCategory

        Task<ExpenseCategoryResponse> GetExpenseCategories(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ExpenseCategory> GetExpenseCategory(long categoryID, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Invoice

        Task<Invoice> CreateInvoice(long? clientId, string currency, DateTime? issuedAt, DateTime? dueAt, string number, string subject, string purchaseOrder, string clientKey, string notes, decimal? tax, decimal? tax2, decimal? taxAmount, decimal? tax2Amount, string kind, string lineItems, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Invoice> GetInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<InvoiceResponse> GetInvoices(DateTime? updatedSince, int? page = 1, int? perPage = 100, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Project

        Task<Project> CreateProject(string name, long? clientId, bool? active, string billBy, string code, string notes, string budgetBy, decimal? budget, bool? billable, decimal? hourlyRate, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? startsOn, DateTime? endsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Project> GetProject(long projectId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ProjectsResponse> GetProjects(long? clientId, DateTime? updatedSince, long? accountId = null, int? page = 1, int? perPage = 100, bool? isActive = true, CancellationToken cancellationToken = default(CancellationToken));

        Task<Project> UpdateProject(long projectId, bool? active, bool? billable, string billBy, decimal? budget, string budgetBy, long? clientId, string code, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? endsOn, decimal? estimate, string estimateBy, decimal? hourlyRate, string name, string notes, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, DateTime? startsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Expenses
        Task<ExpensesResponse> GetExpenses(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, long? projectId = null, long? clientId = null, long? userId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Payment
        Task<PaymentsResponse> GetPayments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? invoiceId = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region task
        Task<Task> GetTask(long taskId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region taskAssignment
        Task<TaskAssignment> GetTaskAssignment(long projectId, long assignmentId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<TaskAssignmentsResponse> GetTaskAssignments(DateTime? updatedSince, int? page, int? perPage, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region UserAssignment
        Task<UserAssignmentsResponse> GetUserAssignments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion
    }
}
