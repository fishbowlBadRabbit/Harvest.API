using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Harvest.Api.IntegrationScripts.HarvestApi
{
    [TestClass]
    public class HarvestClientTests
    {
        #region Property        
        public string AccessToken
        {
            get
            {
                return ConfigurationManager.AppSettings["Harvest_AccessToken"];
            }
        }

        public string UserAgent
        {
            get
            {
                return ConfigurationManager.AppSettings["Harvest_UserAgent"];
            }
        }

        public int AccountId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Harvest_AccountId"]);
            }
        }

        public int TimeEntryId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_TimeEntryId_TestValue"]);
            }
        }

        public int UserId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_UserId_TestValue"]);
            }
        }

        public int ClientId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_ClientId_TestValue"]);
            }
        }

        public int CategoryId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_CategoryId_TestValue"]);
            }
        }

        public int InvoiceId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_InvoiceId_TestValue"]);
            }
        }

        public int ProjectId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_ProjectId_TestValue"]);
            }
        }

        public int TaskId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_TaskId_TestValue"]);
            }
        }

        public int AssignmentId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ScriptTest_AssignmentId_TestValue"]);
            }
        }
        
        #endregion

        #region TestMethod

        [TestMethod]
        public void HarvestClientTests_GetAccountsAsync_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetAccountsAsync().Result;
            Assert.IsTrue(result.Accounts.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetTimeEntriesAsync_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTimeEntriesAsync(accountId:AccountId).Result;
            Assert.IsTrue(result.TimeEntries.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetTimeEntryAsync_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTimeEntryAsync(TimeEntryId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == TimeEntryId);
        }

        [TestMethod]
        public void HarvestClientTests_RestartTimeEntryAsync_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.RestartTimeEntryAsync(TimeEntryId, accountId: AccountId).Result;
            Assert.IsTrue(result.StartedTime != null);
        }

        [TestMethod]
        public void HarvestClientTests_StopTimeEntryAsync_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.StopTimeEntryAsync(TimeEntryId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == TimeEntryId); 
        }

        [TestMethod]
        public void HarvestClientTests_GetProjectAssignmentsAsync_AssertPopulated()
        {          
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetProjectAssignmentsAsync(accountId: AccountId).Result;
            Assert.IsTrue(result.ProjectAssignments.Count() > 0); 
        }

        [TestMethod]
        public void HarvestClientTests_GetProjectsAsync_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetProjectsAsync(accountId: AccountId).Result; 
            Assert.IsTrue(result.Projects.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetTasksAsync_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTasksAsync(accountId: AccountId).Result;
            Assert.IsTrue(result.Tasks.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetMe_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetMe(accountId: AccountId).Result;
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void HarvestClientTests_GetUsers_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetUsers(accountId: AccountId).Result;
            Assert.IsTrue(result.Users.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetUser_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetUser(UserId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == UserId);
        }

        [TestMethod]
        public void HarvestClientTests_GetClients_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetClients(accountId: AccountId).Result; 
            Assert.IsTrue(result.Clients.Count() > 0);           
        }

        [TestMethod]
        public void HarvestClientTests_GetClient_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetClient(ClientId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == ClientId);            
        }

        [TestMethod]
        public void HarvestClientTests_GetContacts_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetContacts(accountId: AccountId).Result; 
            Assert.IsTrue(result.Contacts.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetExpenseCategories_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetExpenseCategories(accountId: AccountId).Result; 
            Assert.IsTrue(result.ExpenseCategories.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetExpenseCategory_AssertPopulated()
        {          
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetExpenseCategory(CategoryId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id== CategoryId);
        }

        [TestMethod]
        public void HarvestClientTests_GetInvoices_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetInvoices(accountId: AccountId).Result;
            Assert.IsTrue(result.Invoices.Count() >0);
        }

        [TestMethod]
        public void HarvestClientTests_GetInvoice_AssertPopulated()
        {          
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetInvoice(InvoiceId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == InvoiceId);
        }

        [TestMethod]
        public void HarvestClientTests_GetProjects_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetProjects(null,null, accountId: AccountId).Result;
            Assert.IsTrue(result.Projects.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetProject_AssertPopulated()
        {          
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetProject(ProjectId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == ProjectId);
        }

        [TestMethod]
        public void HarvestClientTests_GetExpenses_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetExpenses(accountId: AccountId).Result;
            Assert.IsTrue(result.Expenses.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetPayments_AssertPopulated()
        {          
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetPayments(accountId: AccountId, invoiceId: InvoiceId).Result;
            Assert.IsTrue(result.InvoicePayments.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetTask_AssertPopulated()
        {           
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTask(TaskId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == TaskId);
        }
                
        [TestMethod]
        public void HarvestClientTests_GetTaskAssignments_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTaskAssignments(accountId: AccountId).Result;
            Assert.IsTrue(result.TaskAssignments.Count() > 0);
        }

        [TestMethod]
        public void HarvestClientTests_GetTaskAssignment_AssertPopulated()
        {            
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetTaskAssignment(ProjectId, AssignmentId, accountId: AccountId).Result;
            Assert.IsTrue(result.Id == AssignmentId);
        }

        [TestMethod]
        public void HarvestClientTests_GetUserAssignments_AssertPopulated()
        {
            IHarvestClient harvest = new HarvestClient(UserAgent, AccessToken);
            var result = harvest.GetUserAssignments(accountId: AccountId).Result;
            Assert.IsTrue(result.UserAssignments.Count() > 0);
        }
        
        #endregion
    }

    //Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntriesResponse> GetTimeEntriesAsync(long? userId = null, long? clientId = null, long? projectId = null, bool? isBilled = null,
    //  DateTime? updatedSince = null, DateTime? fromDate = null, DateTime? toDate = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> GetTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> RestartTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> StopTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //ThreadingTask DeleteTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> CreateTimeEntryAsync(long projectId, long taskId, DateTime spentDate,
    //  TimeSpan? startedTime = null, TimeSpan? endedTime = null, decimal? hours = null, string notes = null, ExternalReference externalReference = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> UpdateTimeEntryAsync(long entryId,
    //  long? projectId = null, long? taskId = null, DateTime? spentDate = null, TimeSpan? startedTime = null, TimeSpan? endedTime = null,
    //  decimal? hours = null, string notes = null, ExternalReference externalReference = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<ProjectAssignmentsResponse> GetProjectAssignmentsAsync(long? userId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TasksResponse> GetTasksAsync(DateTime? updatedSince = null, int? page = null, int? perPage = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<UserDetails> GetMe(long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<UserDetails> GetUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<UsersResponse> GetUsers(bool? isActive = null, DateTime? updatedSince = null, int? page = null, int? perPage = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> CreateUser(string firstName, string lastName, string email,
    //  string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
    //  bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
    //  bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
    //  bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
    //  decimal? costRate = null, string[] roles = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TimeEntry> UpdateUser(int userId, string firstName = null, string lastName = null, string email = null,
    //  string telephone = null, string timezone = null, bool? hasAccessToAllFutureProjects = null,
    //  bool? isContractor = null, bool? isAdmin = null, bool? isProjectManager = null,
    //  bool? canSeeRates = null, bool? canCreateProjects = null, bool? canCreateInvoices = null,
    //  bool? isActive = null, int? weeklyCapacity = null, decimal? defaultHourlyRate = null,
    //  decimal? costRate = null, string[] roles = null,
    //  long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //ThreadingTask DeleteUser(long userId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //#region Clients

    //Task<Client> CreateClient(string name, string details, bool active, string currency, long? highriseId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<Client> GetClient(long clientId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<ClientsResponse> GetClients(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

    //#endregion

    //#region ClientContacts

    //Task<ClientContactsResponse> GetContacts(DateTime? updatedSince, long? accountId = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default(CancellationToken));

    //#endregion

    //#region ExpenseCategory

    //Task<ExpenseCategoryResponse> GetExpenseCategories(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<ExpenseCategory> GetExpenseCategory(long categoryID, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //#endregion

    //#region Invoice

    //Task<Invoice> CreateInvoice(long? clientId, string currency, DateTime? issuedAt, DateTime? dueAt, string number, string subject, string purchaseOrder, string clientKey, string notes, decimal? tax, decimal? tax2, decimal? taxAmount, decimal? tax2Amount, string kind, string lineItems, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<bool> DeleteInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<Invoice> GetInvoice(long invoiceId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<InvoiceResponse> GetInvoices(DateTime? updatedSince, int? page = 1, int? perPage = 100, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region Project

    //Task<Project> CreateProject(string name, long? clientId, bool? active, string billBy, string code, string notes, string budgetBy, decimal? budget, bool? billable, decimal? hourlyRate, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? startsOn, DateTime? endsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<Project> GetProject(long projectId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<ProjectsResponse> GetProjects(long? clientId, DateTime? updatedSince, long? accountId = null, int? page = 1, int? perPage = 100, bool? isActive = true, CancellationToken cancellationToken = default(CancellationToken));

    //Task<Project> UpdateProject(long projectId, bool? active, bool? billable, string billBy, decimal? budget, string budgetBy, long? clientId, string code, decimal? costBudget, bool? costBudgetIncludeExpenses, DateTime? endsOn, decimal? estimate, string estimateBy, decimal? hourlyRate, string name, string notes, bool? notifyWhenOverBudget, decimal? overBudgetNotificationPercentage, bool? showBudgetToAll, DateTime? startsOn, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region Expenses
    //Task<ExpensesResponse> GetExpenses(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? accountId = null, long? projectId = null, long? clientId = null, long? userId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region Payment
    //Task<PaymentsResponse> GetPayments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, long? invoiceId = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region task
    //Task<Task> GetTask(long taskId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region taskAssignment
    //Task<TaskAssignment> GetTaskAssignment(long projectId, long assignmentId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));

    //Task<TaskAssignmentsResponse> GetTaskAssignments(DateTime? updatedSince, int? page, int? perPage, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion

    //#region UserAssignment
    //Task<UserAssignmentsResponse> GetUserAssignments(DateTime? updatedSince = null, int? page = 1, int? perPage = null, bool? isActive = true, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken));
    //#endregion
}
