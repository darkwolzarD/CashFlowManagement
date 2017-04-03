using CashFlowManagement.ViewModels.RealEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashFlowManagement.EntityModel;

namespace CashFlowManagement.Utilities
{
    public class LoanProcessing
    {
        public static List<LoanInterestTableViewModel> CalculatePaymentsByMonth(List<Loans> list)
        {
            list = list.OrderBy(x => x.Id).ToList();
            List<LoanInterestTableViewModel> result = new List<LoanInterestTableViewModel>();

            double remainLoan = list[0].MortgageValue;
            double numberOfOriginalMonths = (list[0].EndDate.Year - list[0].StartDate.Year) * 12 + list[0].EndDate.Month - list[0].StartDate.Month;
            double monthlyOriginalPayment = list[0].MortgageValue / numberOfOriginalMonths;

            int i = 0;
            double monthlyTotalOriginalPayment = 0;
            for (; i < list.Count - 1; i++)
            {
                double interestPerMonth = list[i].InterestRatePerYear / 1200;

                double numberOfRealMonths = (list[i + 1].StartDate.Year - list[i].StartDate.Year) * 12 + list[i + 1].StartDate.Month - list[i].StartDate.Month;
                for (int j = 1; j <= numberOfRealMonths; j++)
                {
                    
                    LoanInterestTableViewModel item = new LoanInterestTableViewModel
                    {
                        MonthlyOriginalPayment = monthlyOriginalPayment,
                        MonthlyInterestPayment = remainLoan * interestPerMonth,
                        CurrentInterestRatePerYear = list[i].InterestRatePerYear
                    };

                    item.MonthlyTotalPayment = item.MonthlyOriginalPayment + item.MonthlyInterestPayment;
                    item.RemainingLoan = remainLoan;

                    result.Add(item);

                    monthlyTotalOriginalPayment += monthlyOriginalPayment;
                    remainLoan -= monthlyOriginalPayment;
                }                
            }

            double _interestPerMonth = list[i].InterestRatePerYear / 1200;
            double _numberOfRealMonths = (list[i].EndDate.Year - list[i].StartDate.Year) * 12 + list[i].EndDate.Month - list[i].StartDate.Month;
            for (int j = 1; j <= _numberOfRealMonths; j++)
            {
                LoanInterestTableViewModel _item = new LoanInterestTableViewModel
                {
                    MonthlyOriginalPayment = monthlyOriginalPayment,
                    MonthlyInterestPayment = remainLoan * _interestPerMonth,
                    CurrentInterestRatePerYear = list[i].InterestRatePerYear
                };

                _item.MonthlyTotalPayment = _item.MonthlyOriginalPayment + _item.MonthlyInterestPayment;
                _item.RemainingLoan = remainLoan;

                result.Add(_item);

                monthlyTotalOriginalPayment += monthlyOriginalPayment;
                remainLoan -= monthlyOriginalPayment;
            }
            return result;
        }
    }
}