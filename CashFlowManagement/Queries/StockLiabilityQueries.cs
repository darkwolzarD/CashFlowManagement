using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class StockLiabilityQueries
    {
        public static StockLiabilityUpdateViewModel GetViewModelById(int id)
        {
            Entities entities = new Entities();
            var stockLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            StockLiabilityUpdateViewModel liabilityViewModel = new StockLiabilityUpdateViewModel();
            liabilityViewModel.Id = stockLiability.Id;
            liabilityViewModel.Source = stockLiability.Name;
            liabilityViewModel.Value = stockLiability.Value;
            liabilityViewModel.InterestType = stockLiability.InterestType.Value;
            liabilityViewModel.InterestRatePerX = stockLiability.InterestRatePerX;
            liabilityViewModel.InterestRate = stockLiability.InterestRate;
            liabilityViewModel.StartDate = stockLiability.StartDate.Value;
            liabilityViewModel.EndDate = stockLiability.EndDate.Value;
            return liabilityViewModel;
        }

        public static StockLiabilityViewModel CreateViewModel(Liabilities liability)
        {
            DateTime current = DateTime.Now;

            StockLiabilityViewModel liabilityViewModel = new StockLiabilityViewModel();
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

        public static int AddStockLiability(StockLiabilityCreateViewModel model)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            string username = entities.Assets.Where(x => x.Id == model.AssetId).FirstOrDefault().Username;
            var transaction = entities.StockTransactions.Where(x => x.AssetId == model.AssetId).FirstOrDefault();

            Liabilities liability = new Liabilities();
            liability.Name = model.Source;
            liability.Value = model.Value.Value;
            liability.InterestType = model.InterestType;
            liability.InterestRate = model.InterestRate.Value;
            liability.InterestRatePerX = model.InterestRatePerX;
            liability.StartDate = model.StartDate.Value;
            liability.EndDate = model.EndDate.Value;
            liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.STOCK;
            liability.CreatedDate = current;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;
            liability.TransactionId = transaction.Id;

            entities.Liabilities.Add(liability);
            return entities.SaveChanges();
        }

        public static int UpdateStockLiability(StockLiabilityUpdateViewModel model)
        {
            Entities entities = new Entities();
            var stockLiability = entities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();
            stockLiability.Name = model.Source;
            stockLiability.Value = model.Value.Value;
            stockLiability.InterestType = model.InterestType;
            stockLiability.InterestRatePerX = model.InterestRatePerX;
            stockLiability.InterestRate = model.InterestRate.Value;
            stockLiability.StartDate = model.StartDate.Value;
            stockLiability.EndDate = model.EndDate.Value;
            entities.Liabilities.Attach(stockLiability);
            entities.Entry(stockLiability).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteStockLiability(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var stockLiability = entities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            stockLiability.DisabledDate = current;
            stockLiability.DisabledBy = Constants.Constants.USER;
            return entities.SaveChanges();
        }

        public static double GetLiabilityValueOfStock(int stockId)
        {
            Entities entities = new Entities();
            return entities.Liabilities.Where(x => x.AssetId == stockId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public static double GetTotalLiabilityValueOfLiability(int liabilityid)
        {
            Entities entities = new Entities();
            int stockId = entities.Liabilities.Where(x => x.Id == liabilityid).FirstOrDefault().AssetId.Value;
            return entities.Liabilities.Where(x => x.AssetId == stockId && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
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