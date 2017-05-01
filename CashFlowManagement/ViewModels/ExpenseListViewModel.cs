using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class ExpenseListViewModel
    {
        public List<Expenses> List { get; set; }
        public int Type { get; set; }
        public double TotalMonthlyExpense { get; set; }
        public ExpenseListViewModel()
        {
            List = new List<Expenses>();
        }
    }
}