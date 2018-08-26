using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Expense : BaseModel
    {
        public IdNameModel Client { get; set; }
        public IdNameModel Project { get; set; }
        public IdNameModel ExpenseCategory { get; set; }
        public IdNameModel User { get; set; }
        public IdNameModel UserAssignment { get; set; }
        public IdNameModel Receipt { get; set; }
        public IdNameModel Invoice { get; set; }
        public string Notes { get; set; }
        public bool Billable { get; set; }
        public bool IsClosed { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBilled { get; set; }
        public string LockedReason { get; set; }
        public DateTime SpentDate { get; set; }
    }

    public class ExpenseCategory : BaseModel
    {
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitName { get; set; }
    }

    public class ExpenseCategoryResponse : PagedList
    {
        public ExpenseCategory[] ExpenseCategories { get; set; }
    }

    public class ExpensesResponse : PagedList
    {
        public Expense[] Expenses { get; set; }
    }
}
