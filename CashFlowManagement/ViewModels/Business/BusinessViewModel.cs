using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels.Business
{
    public class BusinessListViewModel
    {
        public List<BusinessInfoViewModel> ListBusinessInfoViewModel { get; set; }
        public BusinessListViewModel()
        {
            this.ListBusinessInfoViewModel = new List<BusinessInfoViewModel>();
        }
    }

    public class LoanViewModel
    {
        public BusinessLoan Loan { get; set; }
        public int TotalPaymentPeriod { get; set; }
        public double MonthlyOriginalPayment { get; set; }
        public double MonthlyInterestPayment { get; set; }
        public double MonthlyPayment { get; set; }
        public double AnnualPayment { get; set; }
        public double RemainedValue { get; set; }

        public double CurrentInterestRate { get; set; }
    }

    public class BusinessInfoViewModel
    {
        public BusinessIncomes Business { get; set; }
        public double AnnualIncome { get; set; }
        public double Yield { get; set; }
        public List<LoanViewModel> ListLoanViewModel { get; set; }
        public double TotalMorgageValue { get; set; }
        public double TotalAnnualPayment { get; set; }
        public double TotalMonthlyPayment { get; set; }
        public double TotalInterestPayment { get; set; }
        public double TotalOriginalPayment { get; set; }
        public double TotalRemainingValue { get; set; }
        public double AverageInterestRate { get; set; }
    }
}