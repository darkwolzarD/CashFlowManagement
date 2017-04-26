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
            Entities entities = new Entities();
            List<AssetViewModel> queryResult = (from asset in entities.Assets
                                                     join income in entities.Incomes on asset.Id equals income.AssetId
                                                     where asset.Username == username && asset.AssetType == type
                                                     && !asset.DisabledDate.HasValue && !income.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset, Income = income }).ToList();
            AssetListViewModel result = new AssetListViewModel
            {
                Type = type,
                List = queryResult
            };

            result = LiabilityQueries.GetLiabilityListViewModelByAssetListViewModel(result);

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
            Assets asset = model.Asset;
            asset.Username = username;
            asset.CreatedDate = DateTime.Now;
            asset.AssetType = type;
            asset.CreatedBy = Constants.Constants.USER;

            Incomes income = model.Income;
            income.CreatedDate = DateTime.Now;
            income.IncomeType = type;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;
            asset.Incomes.Add(income);

            entities.Assets.Add(asset);

            string sType = string.Empty;
            
            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.ADD, "tài sản \"" + model.Asset.AssetName + "\"", username, model.Asset.Value);

            entities.Log.Add(log);
            int result = entities.SaveChanges();
            return result;
        }

        public static int BuyAsset(AssetViewModel model, string username)
        {
            Entities entities = new Entities();
            Assets asset = model.Asset;
            asset.Username = username;
            asset.CreatedDate = DateTime.Now;
            asset.AssetType = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE;
            asset.CreatedBy = Constants.Constants.USER;

            Incomes income = model.Income;
            income.CreatedDate = DateTime.Now;
            income.IncomeType = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;
            asset.Incomes.Add(income);

            entities.Assets.Add(asset);

            string sType = string.Empty;

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "tài sản \"" + model.Asset.AssetName + "\"", username, model.Asset.Value);

            entities.Log.Add(log);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateAsset(AssetViewModel model)
        {
            Entities entities = new Entities();

            Assets updated_asset = model.Asset;
            updated_asset.CreatedDate = DateTime.Now;
            updated_asset.CreatedBy = Constants.Constants.USER;

            Incomes updated_income = model.Income;
            updated_income.CreatedDate = DateTime.Now;
            updated_income.CreatedBy = Constants.Constants.USER;
            updated_income.Username = model.Asset.Username;
            updated_asset.Incomes.Add(updated_income);

            IQueryable<Liabilities> liabilities = entities.Liabilities.Where(x => x.AssetId == model.Asset.Id && !x.DisabledDate.HasValue);
            foreach (var liability in liabilities)
            {
                liability.AssetId = 0;
                updated_asset.Liabilities.Add(liability);
            }

            DeleteAsset(model.Asset.Id);

            entities.Assets.Add(updated_asset);

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.UPDATE, "tài sản \"" + model.Asset.AssetName + "\"", model.Asset.Username, model.Asset.Value);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteAsset(int id)
        {
            Entities entities = new Entities();
            Assets asset = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            asset.DisabledDate = DateTime.Now;
            asset.DisabledBy = Constants.Constants.USER;

            entities.Assets.Attach(asset);
            var entry = entities.Entry(asset);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.DisabledBy).IsModified = true;

            Incomes income = entities.Incomes.Where(x => x.AssetId == id && !x.DisabledDate.HasValue).FirstOrDefault();
            income.DisabledDate = DateTime.Now;
            income.DisabledBy = Constants.Constants.USER;

            entities.Incomes.Attach(income);
            var entry_2 = entities.Entry(income);
            entry_2.Property(x => x.DisabledDate).IsModified = true;
            entry_2.Property(x => x.DisabledBy).IsModified = true;

            IQueryable<Liabilities> liabilities = entities.Liabilities.Where(x => x.AssetId == asset.Id && !x.DisabledDate.HasValue);
            foreach (var liability in liabilities)
            {
                liability.DisabledDate = DateTime.Now;
                liability.DisabledBy = Constants.Constants.USER;
                entities.Liabilities.Attach(liability);
                var entry_3 = entities.Entry(liability);
                entry_3.Property(x => x.DisabledDate).IsModified = true;
                entry_3.Property(x => x.DisabledBy).IsModified = true;
            }

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.DELETE, "tài sản \"" + asset.AssetName + "\"", asset.Username, asset.Value);
            entities.Log.Add(log);

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