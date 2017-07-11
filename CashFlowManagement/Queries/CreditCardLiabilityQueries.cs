using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class CreditCardLiabilityQueries
    {
        public static CreditCardLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var creditCardLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            CreditCardLiabilityUpdateViewModel liabilityViewModel = new CreditCardLiabilityUpdateViewModel();
            liabilityViewModel.Id = creditCardLiability.Id;
            liabilityViewModel.Source = creditCardLiability.Name;
            liabilityViewModel.Value = creditCardLiability.Value;
            liabilityViewModel.InterestRate = creditCardLiability.InterestRate;
            liabilityViewModel.Note = creditCardLiability.Note;
            return liabilityViewModel;
        }

        public static CreditCardLiabilityListViewModel GetCreditCardLiabilityByUser(string username)
        {
            Entities entities = new Entities();
            CreditCardLiabilityListViewModel result = new CreditCardLiabilityListViewModel();
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                CreditCardLiabilityViewModel viewModel = CreateViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            result.TotalValue = result.Liabilities.Sum(x => x.Value);
            result.TotalMonthlyPayment = result.Liabilities.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.Liabilities.Sum(x => x.AnnualPayment);
            result.TotalInterestRate = result.TotalMonthlyPayment / result.TotalValue * 100;
            return result;
        }

        public static CreditCardLiabilitySummaryListViewModel GetCreditCardLiabilitySummaryByUser(string username)
        {
            Entities entities = new Entities();
            CreditCardLiabilitySummaryListViewModel result = new CreditCardLiabilitySummaryListViewModel();
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                CreditCardLiabilitySummaryViewModel viewModel = CreateSummaryViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            result.TotalValue = result.Liabilities.Sum(x => x.Value);
            result.TotalMonthlyPayment = result.Liabilities.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.Liabilities.Sum(x => x.AnnualPayment);
            result.TotalInterestRate = result.TotalMonthlyPayment / result.TotalValue * 100;
            return result;
        }

        public static CreditCardLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            CreditCardLiabilityViewModel liabilityViewModel = new CreditCardLiabilityViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.Value = liability.Value;
            liabilityViewModel.InterestRate = liability.InterestRate / 100;
            liabilityViewModel.Note = liability.Note;
            liabilityViewModel.MonthlyPayment = liabilityViewModel.Value * liabilityViewModel.InterestRate / 100;
            liabilityViewModel.AnnualPayment = liabilityViewModel.MonthlyPayment * 12;
            return liabilityViewModel;
        }

        public static CreditCardLiabilitySummaryViewModel CreateSummaryViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            CreditCardLiabilitySummaryViewModel liabilityViewModel = new CreditCardLiabilitySummaryViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.Value = liability.Value;
            liabilityViewModel.InterestRate = liability.InterestRate / 100;
            liabilityViewModel.Note = liability.Note;
            liabilityViewModel.MonthlyPayment = liabilityViewModel.Value * liabilityViewModel.InterestRate / 100;
            liabilityViewModel.AnnualPayment = liabilityViewModel.MonthlyPayment * 12;
            return liabilityViewModel;
        }

        public static int AddCreditCardLiability(CreditCardLiabilityCreateViewModel model, string username)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Liabilities liability = new Liabilities();
            liability.Name = model.Source;
            liability.Value = model.Value.Value;
            liability.InterestRate = model.InterestRate.Value;
            liability.StartDate = current;
            liability.Note = model.Note;
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD;
            liability.InterestRatePerX = (int)Constants.Constants.INTEREST_RATE_PER.MONTH;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateCreditCardLiability(CreditCardLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var creditCardLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            creditCardLiability.Name = model.Source;
            creditCardLiability.Value = model.Value.Value;
            creditCardLiability.InterestRate = model.InterestRate.Value;
            creditCardLiability.Note = model.Note;
            entities.Liabilities.Attach(creditCardLiability);
            entities.Entry(creditCardLiability).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteCreditCardLiability(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var creditCardLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            creditCardLiability.DisabledDate = current;
            creditCardLiability.DisabledBy = Constants.Constants.USER;
            return entities.SaveChanges();
        }

        public static class Helper
        {
            public static string GetInterestType(int interestType)
            {
                if (interestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    return "Cố định";
                }
                else if (interestType == (int)Constants.Constants.INTEREST_TYPE.REDUCED)
                {
                    return "Giảm dần";
                }
                return string.Empty;
            }
            public static string GetInterestTypePerX(int interestTypePerX)
            {
                if (interestTypePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH)
                {
                    return "Tháng";
                }
                else if (interestTypePerX == (int)Constants.Constants.INTEREST_RATE_PER.YEAR)
                {
                    return "Năm";
                }
                return string.Empty;
            }

            public static int CalculateTimePeriod(DateTime startDate, DateTime endDate)
            {
                if (endDate >= startDate)
                {
                    int period = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
                    if ((endDate.Day - startDate.Day) / 30 >= 0.5)
                    {
                        period += 1;
                    }
                    return period;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}