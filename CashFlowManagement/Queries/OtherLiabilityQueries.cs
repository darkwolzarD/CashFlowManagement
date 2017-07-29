using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class OtherLiabilityQueries
    {
        public static OtherLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var carLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            OtherLiabilityUpdateViewModel liabilityViewModel = new OtherLiabilityUpdateViewModel();
            liabilityViewModel.Id = carLiability.Id;
            liabilityViewModel.Source = carLiability.Name;
            liabilityViewModel.Purpose = carLiability.Purpose;
            liabilityViewModel.Value = carLiability.Value;
            liabilityViewModel.InterestType = carLiability.InterestType.Value;
            liabilityViewModel.InterestRatePerX = carLiability.InterestRatePerX;
            liabilityViewModel.InterestRate = carLiability.InterestRate;
            liabilityViewModel.StartDate = carLiability.StartDate.Value;
            liabilityViewModel.EndDate = carLiability.EndDate.Value;
            liabilityViewModel.Note = carLiability.Note;
            return liabilityViewModel;
        }

        public static OtherLiabilityListViewModel GetOtherLiabilityByUser(string username)
        {
            Entities entities = new Entities();
            OtherLiabilityListViewModel result = new OtherLiabilityListViewModel();
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                OtherLiabilityViewModel viewModel = CreateViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            result.TotalLiabilityValue = result.Liabilities.Sum(x => x.Value.Value);
            result.TotalInterestPayment = result.Liabilities.Sum(x => x.MonthlyInterestPayment);
            result.TotalOriginalPayment = result.Liabilities.Sum(x => x.MonthlyOriginalPayment);
            result.TotalPayment = result.Liabilities.Sum(x => x.TotalMonthlyPayment);
            result.TotalRemainedValue = result.Liabilities.Sum(x => x.RemainedValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.Liabilities.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            return result;
        }

        public static OtherLiabilitySummaryListViewModel GetOtherLiabilitySummaryByUser(string username) 
        {
            Entities entities = new Entities();
            OtherLiabilitySummaryListViewModel result = new OtherLiabilitySummaryListViewModel();
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                OtherLiabilitySummaryViewModel viewModel = CreateSummaryViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            result.TotalLiabilityValue = result.Liabilities.Sum(x => x.Value.Value);
            result.TotalInterestPayment = result.Liabilities.Sum(x => x.MonthlyInterestPayment);
            result.TotalOriginalPayment = result.Liabilities.Sum(x => x.MonthlyOriginalPayment);
            result.TotalPayment = result.Liabilities.Sum(x => x.TotalMonthlyPayment);
            result.TotalRemainedValue = result.Liabilities.Sum(x => x.RemainedValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.Liabilities.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            return result;
        }

        public static OtherLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            OtherLiabilityViewModel liabilityViewModel = new OtherLiabilityViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.Purpose = liability.Purpose;
            liabilityViewModel.Value = liability.Value;
            liabilityViewModel.InterestType = Helper.GetInterestType(liability.InterestType.Value);
            liabilityViewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
            liabilityViewModel.InterestRate = liability.InterestRate / 100;
            liabilityViewModel.StartDate = liability.StartDate.Value;
            liabilityViewModel.EndDate = liability.EndDate.Value;
            liabilityViewModel.Note = liability.Note;
            liabilityViewModel.PaymentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, liabilityViewModel.EndDate.Value);

            if (liabilityViewModel.StartDate <= current && current <= liabilityViewModel.EndDate)
            {
                int currentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, DateTime.Now);
                double interestRate = liability.InterestRatePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH ? liability.InterestRate / 100 : liability.InterestRate / 1200;
                liabilityViewModel.OriginalInterestPayment = liabilityViewModel.Value.Value * interestRate;
                //Fixed interest type
                if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.Value.Value * interestRate;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = liabilityViewModel.TotalMonthlyPayment * currentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                    liabilityViewModel.Status = "Đang nợ";
                    liabilityViewModel.StatusCode = "label-success";
                }
                //Reduced interest type
                else
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                    liabilityViewModel.MonthlyInterestPayment = (liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * currentPeriod) * interestRate;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = interestRate * (currentPeriod * liabilityViewModel.Value.Value + (currentPeriod * (currentPeriod + 1) / 2) * liabilityViewModel.MonthlyOriginalPayment);
                    liabilityViewModel.Status = "Đang nợ";
                    liabilityViewModel.StatusCode = "label-success";
                }
            }
            else
            {
                liabilityViewModel.MonthlyOriginalPayment = 0;
                liabilityViewModel.MonthlyInterestPayment = 0;
                liabilityViewModel.TotalMonthlyPayment = 0;
                liabilityViewModel.TotalPayment = 0;
                liabilityViewModel.RemainedValue = 0;
                if (liabilityViewModel.EndDate < current)
                {
                    liabilityViewModel.StatusCode = "label-warning";
                    liabilityViewModel.Status = "Đã trả hết nợ";
                }
                else
                {
                    liabilityViewModel.StatusCode = "label-danger";
                    liabilityViewModel.Status = "Chưa tới kì hạn";
                }
            }

            return liabilityViewModel;
        }

        public static OtherLiabilitySummaryViewModel CreateSummaryViewModel(Liabilities liability) 
        {
            DateTime current = DateTime.Now;

            OtherLiabilitySummaryViewModel liabilityViewModel = new OtherLiabilitySummaryViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.Purpose = liability.Purpose;
            liabilityViewModel.Value = liability.Value;
            liabilityViewModel.InterestType = Helper.GetInterestType(liability.InterestType.Value);
            liabilityViewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
            liabilityViewModel.InterestRate = liability.InterestRate / 100;
            liabilityViewModel.StartDate = liability.StartDate.Value;
            liabilityViewModel.EndDate = liability.EndDate.Value;
            liabilityViewModel.Note = liability.Note;
            liabilityViewModel.PaymentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, liabilityViewModel.EndDate.Value);

            if (liabilityViewModel.StartDate <= current && current <= liabilityViewModel.EndDate)
            {
                int currentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, DateTime.Now);
                double interestRate = liability.InterestRatePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH ? liability.InterestRate / 100 : liability.InterestRate / 1200;
                liabilityViewModel.OriginalInterestPayment = liabilityViewModel.Value.Value * interestRate;
                //Fixed interest type
                if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.Value.Value * interestRate;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = liabilityViewModel.TotalMonthlyPayment * currentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                    liabilityViewModel.Status = "Đang nợ";
                    liabilityViewModel.StatusCode = "label-success";
                }
                //Reduced interest type
                else
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                    liabilityViewModel.MonthlyInterestPayment = (liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * currentPeriod) * interestRate;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = interestRate * (currentPeriod * liabilityViewModel.Value.Value + (currentPeriod * (currentPeriod + 1) / 2) * liabilityViewModel.MonthlyOriginalPayment);
                    liabilityViewModel.Status = "Đang nợ";
                    liabilityViewModel.StatusCode = "label-success";
                }
            }
            else
            {
                liabilityViewModel.MonthlyOriginalPayment = 0;
                liabilityViewModel.MonthlyInterestPayment = 0;
                liabilityViewModel.TotalMonthlyPayment = 0;
                liabilityViewModel.TotalPayment = 0;
                liabilityViewModel.RemainedValue = 0;
                if (liabilityViewModel.EndDate < current)
                {
                    liabilityViewModel.StatusCode = "label-warning";
                    liabilityViewModel.Status = "Đã trả hết nợ";
                }
                else
                {
                    liabilityViewModel.StatusCode = "label-danger";
                    liabilityViewModel.Status = "Chưa tới kì hạn";
                }
            }

            return liabilityViewModel;
        }

        public static int AddOtherLiability(OtherLiabilityCreateViewModel model, string username)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Liabilities liability = new Liabilities();
            liability.Name = model.Source;
            liability.Value = model.Value.Value;
            liability.Purpose = model.Purpose;
            liability.InterestType = model.InterestType;
            liability.InterestRate = model.InterestRate.Value;
            liability.InterestRatePerX = model.InterestRatePerX;
            liability.StartDate = model.StartDate.Value;
            liability.EndDate = model.EndDate.Value;
            liability.Note = model.Note;
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.OTHERS;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateOtherLiability(OtherLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var carLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            carLiability.Name = model.Source;
            carLiability.Value = model.Value.Value;
            carLiability.Purpose = model.Purpose;
            carLiability.InterestType = model.InterestType;
            carLiability.InterestRatePerX = model.InterestRatePerX;
            carLiability.InterestRate = model.InterestRate.Value;
            carLiability.StartDate = model.StartDate.Value;
            carLiability.EndDate = model.EndDate.Value;
            carLiability.Note = model.Note;
            entities.Liabilities.Attach(carLiability);
            entities.Entry(carLiability).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteOtherLiability(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var carLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            carLiability.DisabledDate = current;
            carLiability.DisabledBy = Constants.Constants.USER;
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