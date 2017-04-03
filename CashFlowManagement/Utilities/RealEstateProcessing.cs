using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels.RealEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Utilities
{
    public class RealEstateProcessing
    {
        public static int CalculateTimePeriod(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
        }

        public static LoanViewModel GetLoanViewModel(Loans loan)
        {
            LoanViewModel result = new LoanViewModel();
            
            result.Loan = loan;
            result.TotalPaymentPeriod = CalculateTimePeriod(loan.StartDate, loan.EndDate);
            result.MonthlyOriginalPayment = loan.MortgageValue / result.TotalPaymentPeriod;
            result.CurrentInterestRate = RealEstateQueries.GetCurrentInterestRate(loan.Id);

            int currentPeriod = CalculateTimePeriod(loan.StartDate, DateTime.Now);

            if (currentPeriod > 0)
            {
                result.RemainedValue = loan.MortgageValue - (currentPeriod - 1) * result.MonthlyOriginalPayment;
                result.MonthlyInterestPayment = result.RemainedValue * loan.InterestRatePerYear / 1200;
            }
            else
            {
                result.RemainedValue = loan.MortgageValue;
                result.MonthlyInterestPayment = 0;
            }

            result.MonthlyPayment = result.MonthlyInterestPayment + result.MonthlyOriginalPayment;
            result.AnnualPayment = loan.MortgageValue * loan.InterestRatePerYear / 100;           //chua xu ly//            

            return result;
        }

        public static RealEstateInfoViewModel GetRealEstateInfoViewModel(RealEstateIncomes realEstateIncome)
        {
            RealEstateInfoViewModel result = new RealEstateInfoViewModel();
            result.RealEstate = realEstateIncome;
            result.AnnualRentIncome = realEstateIncome.Income * 12;
            result.RentYield = 100 * result.AnnualRentIncome / realEstateIncome.OriginalValue;
            //result.ListLoanViewModel = GetLoanViewModel(realEstateIncome)            
            List<LoanViewModel> lstLoanViewModel = new List<LoanViewModel>();

            double TotalMorgageValue = 0;

            double TotalInterestPayment = 0;
            double TotalOriginalPayment = 0;
            double TotalMonthlyPayment = 0;
            double TotalAnnualPayment = 0;

            double TotalRemainingValue = 0;
            

            foreach (var item in realEstateIncome.Loans)
            {
                LoanViewModel loanViewModel = GetLoanViewModel(item);
                lstLoanViewModel.Add(loanViewModel);
                TotalMorgageValue += loanViewModel.Loan.MortgageValue;
                TotalInterestPayment += loanViewModel.MonthlyInterestPayment;
                TotalOriginalPayment += loanViewModel.MonthlyOriginalPayment;
                TotalMonthlyPayment += loanViewModel.MonthlyPayment;
                TotalRemainingValue += loanViewModel.RemainedValue;
                TotalAnnualPayment += loanViewModel.AnnualPayment;
            }
            result.ListLoanViewModel = lstLoanViewModel;
            result.TotalMorgageValue = TotalMorgageValue;
            result.TotalInterestPayment = TotalInterestPayment;
            result.TotalOriginalPayment = TotalOriginalPayment;
            result.TotalMonthlyPayment = TotalMonthlyPayment;
            result.TotalRemainingValue = TotalRemainingValue;
            result.AverageInterestRate = 100 * TotalAnnualPayment / TotalMorgageValue;
            result.TotalAnnualPayment = TotalAnnualPayment;
            return result;
        }

        public static RealEstateListViewModel GetRealEstateListViewModel(List<RealEstateIncomes> listRealEstateIncomes)
        {
            RealEstateListViewModel result = new RealEstateListViewModel();
            foreach (var item in listRealEstateIncomes)
            {
                result.ListRealEstateInfoViewModel.Add(GetRealEstateInfoViewModel(item));
            }
            return result;
        }
    }
}