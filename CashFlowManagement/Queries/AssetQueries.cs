using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Queries
{
    public class AssetQueries
    {
        public static AssetListViewModel GetAssetByUser(string username, int type)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            Entities entities = new Entities();
            IQueryable<AssetViewModel> queryResult = from asset in entities.Assets
                                                     join income in entities.Incomes on asset.Id equals income.AssetId
                                                     where asset.Username == username && asset.AssetType == type
                                                     select new AssetViewModel { Asset = asset, Income = income };
            AssetListViewModel result = new AssetListViewModel
            {
                Type = type,
                List = queryResult
            };
            return result;
        }

        public static AssetViewModel GetAssetById(int id)
        {
            Entities entities = new Entities();
            AssetViewModel result = (from asset in entities.Assets
                                     join income in entities.Incomes on asset.Id equals income.AssetId
                                     where asset.Id == id && !asset.DisabledDate.HasValue && !income.DisabledDate.HasValue
                                     select new AssetViewModel { Asset = asset, Income = income }).FirstOrDefault();
            return result;
        }

        public static int CreateAsset(AssetViewModel model, int type, string username)
        {
            Entities entities = new Entities();
            if (type == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
            {
                Assets asset = model.Asset;
                asset.Username = username;
                asset.CreatedDate = DateTime.Now;
                asset.AssetType = type;

                Incomes income = model.Income;
                income.CreatedDate = DateTime.Now;
                income.IncomeType = type;
                income.Username = username;
                asset.Incomes.Add(income);

                entities.Assets.Add(asset);
            }
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateAsset(AssetViewModel model)
        {
            Entities entities = new Entities();
            Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
            asset.DisabledDate = DateTime.Now;
            asset.Incomes.Where(x => !x.DisabledDate.HasValue).FirstOrDefault().DisabledDate = DateTime.Now;

            entities.Assets.Attach(asset);
            var entry = entities.Entry(asset);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.Incomes.Where(m => !m.DisabledDate.HasValue).FirstOrDefault().DisabledDate).IsModified = true;

            Assets updated_asset = new Assets();

            entities.Assets.Add(asset);
            int result = entities.SaveChanges();
            return result;
        }

        //public static int UpdateBankDeposit(BankDepositIncomes data)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes business = entities.BankDepositIncomes.Where(x => x.Id == data.Id).FirstOrDefault();
        //    DateTime current = DateTime.Now;

        //    business.EndDate = new DateTime(current.Year, current.Month, 1);
        //    entities.BankDepositIncomes.Attach(business);
        //    var entry = entities.Entry(business);
        //    entry.Property(x => x.EndDate).IsModified = true;

        //    BankDepositIncomes updated_bankdeposit = new BankDepositIncomes();
        //    updated_bankdeposit.Source = data.Source;
        //    updated_bankdeposit.CapitalValue = data.CapitalValue;
        //    updated_bankdeposit.InterestYield = data.InterestYield;
        //    updated_bankdeposit.ParticipantBank = data.ParticipantBank;
        //    updated_bankdeposit.Note = data.Note;
        //    updated_bankdeposit.StartDate = new DateTime(current.Year, current.Month, 1);
        //    updated_bankdeposit.Username = business.Username;
        //    entities.BankDepositIncomes.Add(updated_bankdeposit);

        //    int result = entities.SaveChanges();
        //    return result;
        //}

        //public static int DeleteBankDeposit(int id)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
        //    DateTime current = DateTime.Now;
        //    deposit.EndDate = new DateTime(current.Year, current.Month, 1);
        //    entities.BankDepositIncomes.Attach(deposit);
        //    var entry = entities.Entry(deposit);
        //    entry.Property(x => x.EndDate).IsModified = true;
        //    int result = entities.SaveChanges();
        //    return result;
        //}

        //public static BankDepositIncomes GetBankDepositById(int id)
        //{
        //    CashFlowManagementEntities entities = new CashFlowManagementEntities();
        //    BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
        //    return deposit;
        //}
    }
}