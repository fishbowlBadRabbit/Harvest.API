﻿using System;
using System.Collections.Generic;
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

        Task<ProjectAssignment> CreateProjectAssignmentAsync(long projectId, long userId, bool? isActive = null, bool? isProjectManager = null, bool? useDefaultRates = null, decimal? hourlyRate = null, decimal? budget = null,
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

        Task<Invoice> GetInvoiceAsync(long invoiceId, long? clientId = null, long? accountId = null,
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

        Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, bool? isActive = null, int? page = null,
            int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Project> UpdateProjectAsync(long projectId, long? clientId = null, string name = null,
            bool? isBillable = null,
            string billBy = "none", string code = null, bool? isFixedFee = null, decimal? hourlyRate = null,
            decimal? budget = null,
            string budgetBy = "none", bool? budgetIsMonthly = null, bool? notifyWhenOverBudget = null,
            bool? overBudgetNotificationPercentage = null,
            bool? showBudgetToAll = null, decimal? costBudget = null, bool? costBudgetIncludeExpenses = null,
            decimal? fee = null, string notes = null, DateTime? startsOn = null, DateTime? endsOn = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Expenses

        Task<ExpensesResponse> GetExpenses(long? userId = null, long? clientId = null, long? projectId = null,
            bool? isBilled = null,
            DateTime? updatedSince = null, DateTime? from = null, DateTime? to = null, int? page = null, int? perPage = null, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Expense> CreateExpense(long projectId, long expenseCategoryId, DateTime spentDate, long? userId = null,
            decimal? units = null, decimal? totalCost = null, string notes = null, bool? billable = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<Expense> UpdateExpense(long expenseId, long projectId, long expenseCategoryId, DateTime spentDate, long? userId = null,
            decimal? units = null, decimal? totalCost = null, string notes = null, bool? billable = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteExpense(long expenseId, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Expense> UpdateExpense(long expenseId, long? projectId = null, long? expenseCategoryId = null,
            DateTime? spentDate = null, int? units = null,
            decimal? totalCost = null, string notes = null, bool? billable = null, long? accountId = null,
            CancellationToken cancellationToken = default(CancellationToken));
        #endregion


        #region Payment
        Task<PaymentsResponse> GetPayments(long invoiceId, DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region task
        Task<Task> GetTaskAsync(long taskId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region taskAssignment

        Task<TaskAssignment> GetTaskAssignmentAsync(long projectId, long taskAssignmentId, DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        Task<TaskAssignmentsResponse> GetTaskAssignmentsAsync(
            DateTime? updatedSince = null, bool? isActive = null, int? page = null, int? perPage = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

        #region UserAssignment
        Task<UserAssignmentsResponse> GetUserAssignmentsAsync(DateTime? updatedSince = null, int? page = 1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
        #endregion
    }
}
