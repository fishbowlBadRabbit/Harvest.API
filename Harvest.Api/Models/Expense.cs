using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Expense : BaseModel
    {
        public Client Client { get; set; }
        public Project Project { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public User User { get; set; }
        public UserAssignment UserAssignment { get; set; }
        public Receipt Receipt { get; set; }
        public Invoice Invoice { get; set; }
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
}
