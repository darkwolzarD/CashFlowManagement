using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CashFlowManagement.Queries.BusinessLiabilityQueries;

namespace CashFlowManagement.Queries
{
    public class BankDepositQueries
    {
        public static BankDepositListViewModel GetBankDepositByUser(string username)
        {
            Entities entities = new Entities();
            var bankDeposits = entities.Assets.Where(x => x.Username.Equals(username) 
                                                && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT 
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.AssetName).ToList();
            BankDepositListViewModel result = new BankDepositListViewModel();
            foreach (var bankDeposit in bankDeposits)
            {
                BankDepositViewModel viewModel = new BankDepositViewModel
                {
                    Id = bankDeposit.Id,
                    Name = bankDeposit.AssetName,
                    Value = bankDeposit.Value,
                    StartDate = bankDeposit.StartDate.Value,
                    EndDate = bankDeposit.EndDate.Value,
                    Income = bankDeposit.Value * bankDeposit.InterestRate.Value / 1200,
                    AnnualIncome = (bankDeposit.Value * bankDeposit.InterestRate.Value / 1200) * 12,
                    InterestRate = bankDeposit.InterestRate.Value / 100,
                    InterestObtainWay = Helper.GetObtainWay(bankDeposit.ObtainedBy.Value),
                    PaymentPeriod = Helper.CalculateTimePeriod(bankDeposit.StartDate.Value, bankDeposit.EndDate.Value),
                    Note = bankDeposit.Note
                };

                result.BankDeposits.Add(viewModel);
            }

            result.TotalValue = result.BankDeposits.Sum(x => x.Value);
            result.TotalIncome = result.BankDeposits.Sum(x => x.Income);
            result.TotalAnnualIncome = result.BankDeposits.Sum(x => x.AnnualIncome);
            result.TotalInterestRate = result.TotalIncome / result.TotalValue * 12;

            return result;
        }

        public static BankDepositUpdateViewModel GetBankDepositById(int id)
        {
            Entities entities = new Entities();
            Assets bankDeposit = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            BankDepositUpdateViewModel model = new BankDepositUpdateViewModel
            {
                Id = bankDeposit.Id,
                Name = bankDeposit.AssetName,
                Value = bankDeposit.Value,
                StartDate = bankDeposit.StartDate.Value,
                EndDate = bankDeposit.EndDate.Value,
                InterestRate = bankDeposit.InterestRate.Value,
                PaymentPeriod = Helper.CalculateTimePeriod(bankDeposit.StartDate.Value, bankDeposit.EndDate.Value),
                Note = bankDeposit.Note
            };
            return model;
        }

        public static BankDepositSummaryListViewModel GetBankDepositSummaryByUser(string username)
        {
            Entities entities = new Entities();
            var bankDeposits = entities.Assets.Where(x => x.Username.Equals(username)
                                                && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.AssetName).ToList();
            BankDepositSummaryListViewModel result = new BankDepositSummaryListViewModel();
            foreach (var bankDeposit in bankDeposits)
            {
                BankDepositSummaryViewModel viewModel = new BankDepositSummaryViewModel
                {
                    Name = bankDeposit.AssetName,
                    Value = bankDeposit.Value,
                    StartDate = bankDeposit.StartDate.Value,
                    EndDate = bankDeposit.EndDate.Value,
                    Income = bankDeposit.Value * bankDeposit.InterestRate.Value / 1200,
                    AnnualIncome = (bankDeposit.Value * bankDeposit.InterestRate.Value / 1200) * 12,
                    InterestRate = bankDeposit.InterestRate.Value / 100,
                    InterestObtainWay = Helper.GetObtainWay(bankDeposit.ObtainedBy.Value),
                    PaymentPeriod = Helper.CalculateTimePeriod(bankDeposit.StartDate.Value, bankDeposit.EndDate.Value),
                    Note = bankDeposit.Note
                };

                result.BankDepositSummaries.Add(viewModel);
            }

            result.TotalValue = result.BankDepositSummaries.Sum(x => x.Value);
            result.TotalIncome = result.BankDepositSummaries.Sum(x => x.Income);
            result.TotalAnnualIncome = result.BankDepositSummaries.Sum(x => x.AnnualIncome);
            result.TotalInterestRate = result.TotalIncome / result.TotalValue;

            return result;
        }

        public static int CreateBankDeposit(BankDepositCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets bankDeposit = new Assets();
            bankDeposit.AssetName = model.Name;
            bankDeposit.Value = model.Value.Value;
            bankDeposit.Note = model.Note;
            bankDeposit.StartDate = model.StartDate.Value;
            bankDeposit.EndDate = model.EndDate.Value;
            bankDeposit.InterestRate = model.InterestRate;
            bankDeposit.ObtainedBy = model.InterestObtainWay;
            bankDeposit.CreatedDate = current;
            bankDeposit.CreatedBy = Constants.Constants.USER;
            bankDeposit.AssetType = (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT;
            bankDeposit.Username = username;

            entities.Assets.Add(bankDeposit);
            return entities.SaveChanges();
        }

        public static int UpdateBankDeposit(BankDepositUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets bankDeposit = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();
            bankDeposit.AssetName = model.Name;
            bankDeposit.Value = model.Value.Value;
            bankDeposit.Note = model.Note;
            bankDeposit.StartDate = model.StartDate.Value;
            bankDeposit.EndDate = model.EndDate.Value;
            bankDeposit.InterestRate = model.InterestRate;
            bankDeposit.ObtainedBy = model.InterestObtainWay;

            entities.Assets.Attach(bankDeposit);
            entities.Entry(bankDeposit).State = System.Data.Entity.EntityState.Modified;

            return entities.SaveChanges();
        }

        public static int DeleteBankDeposit(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets bankDeposit = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            bankDeposit.DisabledDate = current;
            bankDeposit.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(bankDeposit);
            entities.Entry(bankDeposit).State = System.Data.Entity.EntityState.Modified;

            return entities.SaveChanges();
        }

        public static class Helper
        {
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

            public static string GetObtainWay(int obtainCode)
            {
                if(obtainCode == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.START)
                {
                    return "Rút lãi đầu kỳ";
                }
                else if (obtainCode == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.END)
                {
                    return "Rút lãi cuối kỳ";
                }
                else if (obtainCode == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.ORIGIN)
                {
                    return "Lãi nhập gốc";
                }
                return string.Empty;
            }
        }
    }
}