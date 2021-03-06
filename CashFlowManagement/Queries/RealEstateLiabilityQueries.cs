﻿using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Queries
{
    public class RealEstateLiabilityQueries
    {
        public static RealEstateLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var realEstateLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            RealEstateLiabilityUpdateViewModel liabilityViewModel = new RealEstateLiabilityUpdateViewModel();
            liabilityViewModel.Id = realEstateLiability.Id;
            liabilityViewModel.Source = realEstateLiability.Name;
            liabilityViewModel.Value = realEstateLiability.Value;
            liabilityViewModel.InterestType = realEstateLiability.InterestType.Value;
            liabilityViewModel.InterestRatePerX = realEstateLiability.InterestRatePerX;
            liabilityViewModel.InterestRate = realEstateLiability.InterestRate;
            liabilityViewModel.StartDate = realEstateLiability.StartDate.Value;
            liabilityViewModel.EndDate = realEstateLiability.EndDate.Value;
            liabilityViewModel.AssetId = realEstateLiability.AssetId.Value;
            return liabilityViewModel;
        }

        public static RealEstateLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            RealEstateLiabilityViewModel liabilityViewModel = new RealEstateLiabilityViewModel();
            liabilityViewModel.Id = liability.Id;
            liabilityViewModel.Source = liability.Name;
            liabilityViewModel.Value = liability.Value;
            liabilityViewModel.InterestType = Helper.GetInterestType(liability.InterestType.Value);
            liabilityViewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
            liabilityViewModel.InterestRate = liability.InterestRate / 100;
            liabilityViewModel.StartDate = liability.StartDate.Value;
            liabilityViewModel.EndDate = liability.EndDate.Value;
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
                    liabilityViewModel.TotalPayment = Helper.CalculateAnnualPayment(liability);
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
                    liabilityViewModel.TotalPayment = Helper.CalculateAnnualPayment(liability);
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

        public static int AddRealEstateLiability(RealEstateLiabilityCreateViewModel model)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            string username = entities.Assets.Where(x => x.Id == model.AssetId).FirstOrDefault().Username;

            Liabilities liability = new Liabilities();
            liability.Name = model.Source;
            liability.Value = model.Value.Value;
            liability.InterestType = model.InterestType;
            liability.InterestRate = model.InterestRate.Value;
            liability.InterestRatePerX = model.InterestRatePerX;
            liability.StartDate = model.StartDate.Value;
            liability.EndDate = model.EndDate.Value;
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;
            liability.AssetId = model.AssetId;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateRealEstateLiability(RealEstateLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var realEstateLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            realEstateLiability.Name = model.Source;
            realEstateLiability.Value = model.Value.Value;
            realEstateLiability.InterestType = model.InterestType;
            realEstateLiability.InterestRatePerX = model.InterestRatePerX;
            realEstateLiability.InterestRate = model.InterestRate.Value;
            realEstateLiability.StartDate = model.StartDate.Value;
            realEstateLiability.EndDate = model.EndDate.Value;
            entities.Liabilities.Attach(realEstateLiability);
            entities.Entry(realEstateLiability).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteRealEstateLiability(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var realEstateLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            realEstateLiability.DisabledDate = current;
            realEstateLiability.DisabledBy = Constants.Constants.USER;
            return entities.SaveChanges();
        }

        public static double GetLiabilityValueOfRealEstate(int realEstateId)
        {
            Entities entities = new Entities();
            return entities.Liabilities.Where(x => x.AssetId == realEstateId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public static double GetLiabilityValue(int liabilityid)
        {
            Entities entities = new Entities();
            return entities.Liabilities.Where(x => x.Id == liabilityid && !x.DisabledDate.HasValue).FirstOrDefault().Value;
        }

        public static double GetTotalLiabilityValueOfLiability(int liabilityid)
        {
            Entities entities = new Entities();
            int realEstateId = entities.Liabilities.Where(x => x.Id == liabilityid).FirstOrDefault().AssetId.Value;
            return entities.Liabilities.Where(x => x.AssetId == realEstateId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public class Helper
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

            public static double CalculateAnnualPayment(Liabilities liability)
            {
                DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime end = new DateTime(DateTime.Now.Year, 12, 31);
                if (liability.StartDate.Value > start) start = liability.StartDate.Value;
                if (liability.EndDate.Value < end) end = liability.EndDate.Value;
                int period = CalculateTimePeriod(start, end);
                int paymentPeriod = Helper.CalculateTimePeriod(liability.StartDate.Value, liability.EndDate.Value);
                double interestRate = liability.InterestRatePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH ? liability.InterestRate / 100 : liability.InterestRate / 1200;

                if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    double monthlyOriginalPayment = liability.Value / paymentPeriod;
                    double monthlyInterestPayment = liability.Value * interestRate;
                    return (monthlyOriginalPayment + monthlyInterestPayment) * period;
                }
                else
                {
                    double result = 0;
                    DateTime originalStartDate = liability.StartDate.Value;
                    DateTime startDate = liability.StartDate.Value;
                    while (startDate <= liability.EndDate.Value)
                    {
                        int currentPeriod = CalculateTimePeriod(originalStartDate, startDate);
                        if (startDate >= start && startDate <= end)
                        {
                            double monthlyOriginalPayment = liability.Value / paymentPeriod;
                            double remainedValue = liability.Value - monthlyOriginalPayment * (currentPeriod + 1);
                            double monthlyInterestPayment = (liability.Value - monthlyOriginalPayment * currentPeriod) * interestRate;
                            result += monthlyOriginalPayment + monthlyInterestPayment;
                        }
                        startDate = startDate.AddMonths(1);
                    }
                    return result;
                }
            }

            public static double CalculateAnnualPayment(LiabilityCreateViewModel liability)
            {
                DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime end = new DateTime(DateTime.Now.Year, 12, 31);
                if (liability.StartDate.Value > start) start = liability.StartDate.Value;
                if (liability.EndDate.Value < end) end = liability.EndDate.Value;
                int period = CalculateTimePeriod(start, end);
                int paymentPeriod = Helper.CalculateTimePeriod(liability.StartDate.Value, liability.EndDate.Value);
                double interestRate = liability.InterestRatePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH ? liability.InterestRate.Value / 100 : liability.InterestRate.Value / 1200;

                if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    double monthlyOriginalPayment = liability.Value.Value / paymentPeriod;
                    double monthlyInterestPayment = liability.Value.Value * interestRate;
                    return (monthlyOriginalPayment + monthlyInterestPayment) * period;
                }
                else
                {
                    double result = 0;
                    DateTime originalStartDate = liability.StartDate.Value;
                    DateTime startDate = liability.StartDate.Value;
                    while (startDate <= liability.EndDate.Value)
                    {
                        int currentPeriod = CalculateTimePeriod(originalStartDate, startDate);
                        if (startDate >= start && startDate <= end)
                        {
                            double monthlyOriginalPayment = liability.Value.Value / paymentPeriod;
                            double remainedValue = liability.Value.Value - monthlyOriginalPayment * (currentPeriod + 1);
                            double monthlyInterestPayment = (liability.Value.Value - monthlyOriginalPayment * currentPeriod) * interestRate;
                            result += monthlyOriginalPayment + monthlyInterestPayment;
                        }
                        startDate = startDate.AddMonths(1);
                    }
                    return result;
                }
            }
        }
    }
}