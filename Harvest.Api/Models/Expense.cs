﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Expense : BaseModel
    {
        public Client Client { get; set; }
        public ExpenseInvoice Invoice { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public IdNameModel User { get; set; }
        public Receipt Receipt { get; set; }
        public UserAssignment UserAssignment { get; set; }
        public ProjectReference Project { get; set; }
        public bool? Billable { get; set; }
        public DateTime? SpentDate { get; set; }
        public string LockedReason { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBilled { get; set; }
        public bool IsClosed { get; set; }
        public string Notes { get; set; }

        public decimal TotalCost { get; set; }
        public decimal Units { get; set; }
    }

    public class ExpenseCategory : BaseModel
    {
        public string Name { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }

        public bool IsActive { get; set; }
    }

    public class ExpenseInvoice
    {
        public long Id { get; set; }
        public string Number { get; set; }
    }
}
