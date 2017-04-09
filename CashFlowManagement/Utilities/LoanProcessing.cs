using CashFlowManagement.ViewModels.RealEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;

namespace CashFlowManagement.Utilities
{
    public class LoanProcessing
    {
        public static List<LoanInterestTableViewModel> CalculatePaymentsByMonth(List<Loans> list, Loans loan, bool append)
        {
            list = list.OrderBy(x => x.StartDate).ToList();

            Loans parentLoan = list.Where(x => !x.ParentLoanId.HasValue).FirstOrDefault();

            List<LoanInterestTableViewModel> result = new List<LoanInterestTableViewModel>();

            double remainLoan = parentLoan.MortgageValue;
            double numberOfOriginalMonths = (parentLoan.EndDate.Year - parentLoan.StartDate.Year) * 12 + parentLoan.EndDate.Month - parentLoan.StartDate.Month;
            double monthlyOriginalPayment = parentLoan.MortgageValue / numberOfOriginalMonths;
            DateTime startDate = list.Where(x => x.ParentLoanId.HasValue).OrderBy(x => x.StartDate).FirstOrDefault().StartDate;

            double monthlyTotalOriginalPayment = 0;
            foreach (var item in list)
            {
                if (item.ParentLoanId.HasValue)
                {
                    double interestPerMonth = item.InterestRatePerYear / 1200;

                    double numberOfRealMonths = (item.EndDate.Year - item.StartDate.Year) * 12 + item.EndDate.Month - item.StartDate.Month + 1;
                    for (int j = 1; j <= numberOfRealMonths; j++)
                    {
                        LoanInterestTableViewModel model = new LoanInterestTableViewModel();
                        if (append)
                        {
                            if (loan.StartDate <= startDate && startDate <= loan.EndDate)
                            {
                                model.MonthlyOriginalPayment = monthlyOriginalPayment;
                                model.MonthlyInterestPayment = remainLoan * loan.InterestRatePerYear / 1200;
                                model.CurrentInterestRatePerYear = loan.InterestRatePerYear;
                                model.CurrentMonth = startDate;
                                model.Highlight = true;
                            }
                            else
                            {
                                model.MonthlyOriginalPayment = monthlyOriginalPayment;
                                model.MonthlyInterestPayment = remainLoan * interestPerMonth;
                                model.CurrentInterestRatePerYear = item.InterestRatePerYear;
                                model.CurrentMonth = startDate;
                            }
                        }
                        else
                        {
                            model.MonthlyOriginalPayment = monthlyOriginalPayment;
                            model.MonthlyInterestPayment = remainLoan * interestPerMonth;
                            model.CurrentInterestRatePerYear = item.InterestRatePerYear;
                            model.CurrentMonth = startDate;
                        }

                        model.MonthlyTotalPayment = model.MonthlyOriginalPayment + model.MonthlyInterestPayment;
                        model.RemainingLoan = remainLoan;

                        if (!append)
                        {
                            if (loan.StartDate <= startDate && startDate <= loan.EndDate)
                            {
                                result.Add(model);
                            }
                        }
                        else
                        {
                            result.Add(model);
                        }

                        monthlyTotalOriginalPayment += monthlyOriginalPayment;
                        remainLoan -= monthlyOriginalPayment;
                        startDate = startDate.AddMonths(1);
                    }
                }
            }

            return result;
        }

        public static double GetCurrentMonthlyPaymentByUser(string username)
        {
            double result = 0;
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);

            List<RealEstateIncomes> realEstateList = RealEstateQueries.GetRealEstateByUser(username);
            foreach (var realEstate in realEstateList)
            {
                var loanList = realEstate.Loans.Where(x => !x.DisabledDate.HasValue).ToList();
                foreach (var loan in loanList)
                {
                    if (!loan.ParentLoanId.HasValue)
                    {
                        List<LoanInterestTableViewModel> list = CalculatePaymentsByMonth(loanList.Where(x => x.Id == loan.Id || x.ParentLoanId == loan.Id).ToList(), loan, false);
                        foreach (var item in list)
                        {
                            if (item.CurrentMonth.Equals(current))
                                result += item.MonthlyTotalPayment;
                        }
                    }
                }
            }

            return result;
        }
    }
}