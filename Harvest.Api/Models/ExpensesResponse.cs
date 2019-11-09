using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class ExpensesResponse : PagedList
    {
        public Expense[] Expenses { get; set; }
    }
    public class ExpenseCategoriesResponse : IdNameModel
    {
        public ExpenseCategory[] ExpenseCategories { get; set; }
    }
}
