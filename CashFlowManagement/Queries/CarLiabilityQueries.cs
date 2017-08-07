using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class CarLiabilityQueries
    {
        public static CarLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var carLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            CarLiabilityUpdateViewModel liabilityViewModel = new CarLiabilityUpdateViewModel();
            liabilityViewModel.Id = carLiability.Id;
            liabilityViewModel.Source = carLiability.Name;
            liabilityViewModel.LiabilityValue = carLiability.OriginalValue;
            liabilityViewModel.Value = carLiability.Value;
            liabilityViewModel.InterestType = carLiability.InterestType.Value;
            liabilityViewModel.InterestRatePerX = carLiability.InterestRatePerX;
            liabilityViewModel.InterestRate = carLiability.InterestRate;
            liabilityViewModel.StartDate = carLiability.StartDate.Value;
            liabilityViewModel.EndDate = carLiability.EndDate.Value;
            liabilityViewModel.Note = carLiability.Note;
            return liabilityViewModel;
        }

        public static CarLiabilityListViewModel GetCarLiabilityByUser(string username)
        {
            Entities entities = new Entities();
            CarLiabilityListViewModel result = new CarLiabilityListViewModel();
            DateTime current = DateTime.Now;
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                CarLiabilityViewModel viewModel = CreateViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            var lbts = result.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
            result.TotalOriginalValue = lbts.Sum(x => x.LiabilityValue);
            result.TotalLiabilityValue = lbts.Sum(x => x.Value.Value);
            result.TotalInterestPayment = lbts.Sum(x => x.MonthlyInterestPayment);
            result.TotalOriginalPayment = lbts.Sum(x => x.MonthlyOriginalPayment);
            result.TotalPayment = lbts.Sum(x => x.TotalMonthlyPayment);
            result.TotalRemainedValue = lbts.Sum(x => x.RemainedValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? lbts.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static CarLiabilitySummaryListViewModel GetCarLiabilitySummaryByUser(string username)
        {
            Entities entities = new Entities();
            CarLiabilitySummaryListViewModel result = new CarLiabilitySummaryListViewModel();
            DateTime current = DateTime.Now;
            var liabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            foreach (var liability in liabilities)
            {
                CarLiabilitySummaryViewModel viewModel = CreateSummaryViewModel(liability);
                result.Liabilities.Add(viewModel);
            }

            var lbts = result.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
            result.TotalOriginalValue = lbts.Sum(x => x.LiabilityValue);
            result.TotalLiabilityValue = lbts.Sum(x => x.Value.Value);
            result.TotalInterestPayment = lbts.Sum(x => x.MonthlyInterestPayment);
            result.TotalOriginalPayment = lbts.Sum(x => x.MonthlyOriginalPayment);
            result.TotalPayment = lbts.Sum(x => x.TotalMonthlyPayment);
            result.TotalRemainedValue = lbts.Sum(x => x.RemainedValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? lbts.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            return result;
        }

        public static CarLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            CarLiabilityViewModel liabilityViewModel = new CarLiabilityViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.LiabilityValue = liability.OriginalValue.Value;
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
                    liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability); 
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
                    liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability);
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

        public static CarLiabilitySummaryViewModel CreateSummaryViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            CarLiabilitySummaryViewModel liabilityViewModel = new CarLiabilitySummaryViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.LiabilityValue = liability.OriginalValue.Value;
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
                    liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability);
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
                    liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability);
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

        public static int AddCarLiability(CarLiabilityCreateViewModel model, string username)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Liabilities liability = new Liabilities();
            liability.Name = model.Source;
            liability.Value = model.Value.Value;
            liability.OriginalValue = model.LiabilityValue.Value;
            liability.InterestType = model.InterestType;
            liability.InterestRate = model.InterestRate.Value;
            liability.InterestRatePerX = model.InterestRatePerX;
            liability.StartDate = model.StartDate.Value;
            liability.EndDate = model.EndDate.Value;
            liability.Note = model.Note;
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.CAR;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateCarLiability(CarLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var carLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            carLiability.Name = model.Source;
            carLiability.Value = model.Value.Value;
            carLiability.OriginalValue = model.LiabilityValue.Value;
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

        public static int DeleteCarLiability(int id)
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