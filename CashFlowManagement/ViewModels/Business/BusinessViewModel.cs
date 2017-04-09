using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels.Business
{
    public class BusinessViewModel
    {
        public BusinessIncomes Business { get; set; }
        public int Period { get; set; }
        public double MonthlyInterestPayment { get; set; }
        public double MonthlyOriginalPayment { get; set; }
        public double MonthlyTotalPayment { get; set; }
        public double RemainLoan { get; set; }

        public BusinessViewModel()
        {
            Business = new BusinessIncomes();
        }
    }

    public class BusinessListViewModel
    {
        public List<BusinessViewModel> BusinessList { get; set; }
        public double TotalMonthlyIncome { get; set; }
        public double TotalAnnualIncome { get; set; }
        public double TotalCapitalValue { get; set; }
        public double TotalLoanValue { get; set; }
        public double TotalExpenseInterest { get; set; }
        public double TotalPeriod { get; set; }
        public double TotalInterestPayment { get; set; }
        public double TotalOriginalPayment { get; set; }
        public double TotalTotalPayment { get; set; }
        public double TotalRemainLoan { get; set; }

        public BusinessListViewModel()
        {
            BusinessList = new List<BusinessViewModel>();
        }
    }
}