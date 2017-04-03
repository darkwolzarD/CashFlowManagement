using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels.RealEstate
{
    public class RealEstateListViewModel
    {
        public List<RealEstateInfoViewModel> ListRealEstateInfoViewModel { get; set; }
        public RealEstateListViewModel()
        {
            this.ListRealEstateInfoViewModel = new List<RealEstateInfoViewModel>();
        }
    }

    public class LoanViewModel
    {
        public Loans Loan { get; set; }
        public int TotalPaymentPeriod { get; set; }
        public double MonthlyOriginalPayment { get; set; }
        public double MonthlyInterestPayment { get; set; }
        public double MonthlyPayment { get; set; }
        public double AnnualPayment { get; set; }
        public double RemainedValue { get; set; }

        public double CurrentInterestRate { get; set; }
    }

    public class RealEstateInfoViewModel
    {
        public RealEstateIncomes RealEstate { get; set; }
        public double AnnualRentIncome { get; set; } 
        public double RentYield { get; set; }
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