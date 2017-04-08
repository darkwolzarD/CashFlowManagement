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

        public static LoanViewModel GetLoanViewModel(Loans loan, List<Loans> loans)
        {
            LoanViewModel result = new LoanViewModel();
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);

            Loans parentLoan = loans.Where(x => !x.ParentLoanId.HasValue).FirstOrDefault();
            
            result.Loan = loan;
            result.TotalPaymentPeriod = CalculateTimePeriod(loan.StartDate, loan.EndDate) + 1;
            if (loan.ParentLoanId.HasValue)
            {
                result.CurrentInterestRate = loan.InterestRatePerYear;
            }
            else
            {
                result.CurrentInterestRate = RealEstateQueries.GetCurrentInterestRate(parentLoan.Id);
            }
            if (loan.StartDate <= current && current <= loan.EndDate)
            {
                result.MonthlyOriginalPayment = loan.MortgageValue / CalculateTimePeriod(parentLoan.StartDate, parentLoan.EndDate);

                int currentPeriod = CalculateTimePeriod(parentLoan.StartDate, DateTime.Now);

                if (currentPeriod > 0)
                {
                    //result.RemainedValue = loan.MortgageValue - (currentPeriod - 1) * result.MonthlyOriginalPayment;
                    result.RemainedValue = loan.MortgageValue - currentPeriod * result.MonthlyOriginalPayment;
                    result.MonthlyInterestPayment = result.RemainedValue * result.CurrentInterestRate / 1200;
                }
                else
                {
                    result.RemainedValue = loan.MortgageValue;
                    result.MonthlyInterestPayment = 0;
                }

                result.MonthlyPayment = result.MonthlyInterestPayment + result.MonthlyOriginalPayment;
                result.AnnualPayment = loan.MortgageValue * result.CurrentInterestRate / 100;           //chua xu ly// 
            }
            else
            {
                result.MonthlyInterestPayment = 0;
                result.MonthlyOriginalPayment = 0;
                result.MonthlyPayment = 0;
                result.AnnualPayment = 0;
                result.RemainedValue = 0;
            }           

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
                List<Loans> list = new List<Loans>();
                if(item.ParentLoanId.HasValue)
                {
                    list = realEstateIncome.Loans.Where(x => x.Id == item.ParentLoanId || x.ParentLoanId == item.ParentLoanId).ToList();
                }
                else
                {
                    list = realEstateIncome.Loans.Where(x => x.Id == item.Id || x.ParentLoanId == item.Id).ToList(); ;
                }
                LoanViewModel loanViewModel = GetLoanViewModel(item, list);
                lstLoanViewModel.Add(loanViewModel);
                if (!item.ParentLoanId.HasValue)
                {
                    TotalMorgageValue += loanViewModel.Loan.MortgageValue;
                    TotalInterestPayment += loanViewModel.MonthlyInterestPayment;
                    TotalOriginalPayment += loanViewModel.MonthlyOriginalPayment;
                    TotalMonthlyPayment += loanViewModel.MonthlyPayment;
                    TotalRemainingValue += loanViewModel.RemainedValue;
                    TotalAnnualPayment += loanViewModel.AnnualPayment;
                }
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