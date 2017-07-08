using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class BusinessLiabilityQueries
    {
        public static BusinessLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var businessLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            BusinessLiabilityUpdateViewModel liabilityViewModel = new BusinessLiabilityUpdateViewModel();
            liabilityViewModel.Id = businessLiability.Id;
            liabilityViewModel.Source = businessLiability.Name;
            liabilityViewModel.Value = businessLiability.Value;
            liabilityViewModel.InterestType = businessLiability.InterestType.Value;
            liabilityViewModel.InterestRatePerX = businessLiability.InterestRatePerX;
            liabilityViewModel.InterestRate = businessLiability.InterestRate;
            liabilityViewModel.StartDate = businessLiability.StartDate.Value;
            liabilityViewModel.EndDate = businessLiability.EndDate.Value;
            return liabilityViewModel;
        }

        public static BusinessLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            BusinessLiabilityViewModel liabilityViewModel = new BusinessLiabilityViewModel();
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
                //Fixed interest type
                if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.Value.Value * liabilityViewModel.InterestRate.Value / 12;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = liabilityViewModel.TotalMonthlyPayment * currentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.TotalPayment;
                    liabilityViewModel.Status = "Đang nợ";
                    liabilityViewModel.StatusCode = "label-success";
                }
                //Reduced interest type
                else
                {
                    liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                    liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * currentPeriod;
                    liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.RemainedValue * liabilityViewModel.InterestRate.Value / 12;
                    liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                    liabilityViewModel.TotalPayment = liabilityViewModel.InterestRate.Value / 12 * (currentPeriod * liabilityViewModel.Value.Value + currentPeriod * (currentPeriod + 1) / 2 * liabilityViewModel.MonthlyOriginalPayment);
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
                if(liabilityViewModel.EndDate < current)
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

        public static int AddBusinessLiability(BusinessLiabilityCreateViewModel model)
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
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.BUSINESS;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;
            liability.AssetId = model.AssetId;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateBusinessLiability(BusinessLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var businessLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            businessLiability.Name = model.Source;
            businessLiability.Value = model.Value.Value;
            businessLiability.InterestType = model.InterestType;
            businessLiability.InterestRatePerX = model.InterestRatePerX;
            businessLiability.InterestRate = model.InterestRate.Value;
            businessLiability.StartDate = model.StartDate.Value;
            businessLiability.EndDate = model.EndDate.Value;
            entities.Liabilities.Attach(businessLiability);
            entities.Entry(businessLiability).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteBusinessLiability(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var businessLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            businessLiability.DisabledDate = current;
            businessLiability.DisabledBy = Constants.Constants.USER;
            return entities.SaveChanges();
        }

        public static double GetLiabilityValueOfBusiness(int businessId)
        {
            Entities entities = new Entities();
            return entities.Liabilities.Where(x => x.AssetId == businessId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public static double GetTotalLiabilityValueOfLiability(int liabilityid)
        {
            Entities entities = new Entities();
            int businessId = entities.Liabilities.Where(x => x.Id == liabilityid).FirstOrDefault().AssetId.Value;
            return entities.Liabilities.Where(x => x.AssetId == businessId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
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
                    if((endDate.Day - startDate.Day) / 30 >= 0.5)
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