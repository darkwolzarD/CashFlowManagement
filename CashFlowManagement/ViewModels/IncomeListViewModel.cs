using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class IncomeListViewModel
    {
        public List<Incomes> List { get; set; }
        public int Type { get; set; }
        public double TotalMonthlyIncome { get; set; }
        public IncomeListViewModel()
        {
            List = new List<Incomes>();
        }
    }
}