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
            List<AssetViewModel> queryResult = type == (int)Constants.Constants.ASSET_TYPE.INSURANCE ?
                                                    (from asset in entities.Assets
                                                     join liability in entities.Liabilities on asset.Id equals liability.AssetId
                                                     where asset.Username == username && asset.AssetType == type
                                                     && !asset.DisabledDate.HasValue && !liability.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset, Liability = liability }).ToList()
                                                    :
                                                    (from asset in entities.Assets
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
            var type = entities.Assets.Where(x => x.Id == id).FirstOrDefault().AssetType;
            AssetViewModel result = type == (int)Constants.Constants.ASSET_TYPE.INSURANCE ?
                                                    (from asset in entities.Assets
                                                     join liability in entities.Liabilities on asset.Id equals liability.AssetId
                                                     where asset.Id == id && !asset.DisabledDate.HasValue && !liability.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset, Liability = liability }).FirstOrDefault()
                                                    :
                                                    (from asset in entities.Assets
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
            if (type == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE)
            {
                asset.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.CREATE;
            }

            if (type == (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Liabilities liability = new Liabilities
                {
                    Name = "Nợ bảo hiểm " + model.Asset.AssetName,
                    Value = model.Liability.Value,
                    InterestRate = 0,
                    StartDate = model.Liability.StartDate,
                    EndDate = model.Liability.EndDate,
                    LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.INSURANCE,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Constants.Constants.USER,
                    Username = username
                };
                asset.Liabilities.Add(liability);
            }
            else
            {
                Incomes income = model.Income;
                income.CreatedDate = DateTime.Now;
                income.IncomeType = type;
                income.Username = username;
                income.CreatedBy = Constants.Constants.USER;
                asset.Incomes.Add(income);
            }

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

            List<Liabilities> childLiabilities = new List<Liabilities>();
            foreach (var liability in model.Asset.Liabilities)
            {
                liability.Username = username;
                liability.CreatedDate = DateTime.Now;
                liability.CreatedBy = Constants.Constants.USER;
                liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE;
                Liabilities childLiability = new Liabilities();
                childLiability.Name = liability.Name;
                childLiability.Value = liability.Value;
                childLiability.InterestRate = liability.InterestRate;
                childLiability.StartDate = liability.StartDate;
                childLiability.EndDate = liability.EndDate;
                childLiability.LiabilityType = liability.LiabilityType;
                childLiability.CreatedDate = liability.CreatedDate;
                childLiability.CreatedBy = Constants.Constants.USER;
                childLiability.AssetId = liability.AssetId;
                childLiability.Liabilities1.Add(liability);
                childLiability.Username = username;
                childLiability.InterestType = liability.InterestType;
                childLiabilities.Add(childLiability);
            }

            foreach (var liability in childLiabilities)
            {
                model.Asset.Liabilities.Add(liability);
            }

            Assets asset = model.Asset;
            asset.Username = username;
            asset.CreatedDate = DateTime.Now;
            asset.AssetType = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE;
            asset.CreatedBy = Constants.Constants.USER;
            asset.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.BUY;

            Incomes income = model.Income;
            income.CreatedDate = DateTime.Now;
            income.IncomeType = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;
            asset.Incomes.Add(income);

            Assets available_money = new Assets();
            available_money.AssetName = "Tiền mua " + model.Asset.AssetName;
            available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
            available_money.CreatedBy = Constants.Constants.USER;
            available_money.CreatedDate = DateTime.Now;
            available_money.Value = model.BuyAmount * (-1);
            available_money.Username = username;

            entities.Assets.Add(asset);
            entities.Assets.Add(available_money);

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

            if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Liabilities liability = new Liabilities
                {
                    Name = "Nợ bảo hiểm " + model.Asset.AssetName,
                    Value = model.Liability.Value,
                    InterestRate = 0,
                    StartDate = model.Liability.StartDate,
                    EndDate = model.Liability.EndDate,
                    LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.INSURANCE,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Constants.Constants.USER,
                    Username = model.Asset.Username
                };
                updated_asset.Liabilities.Add(liability);
            }
            else
            {
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

            if (asset.AssetType != (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Incomes income = entities.Incomes.Where(x => x.AssetId == id && !x.DisabledDate.HasValue).FirstOrDefault();
                income.DisabledDate = DateTime.Now;
                income.DisabledBy = Constants.Constants.USER;

                entities.Incomes.Attach(income);
                var entry_2 = entities.Entry(income);
                entry_2.Property(x => x.DisabledDate).IsModified = true;
                entry_2.Property(x => x.DisabledBy).IsModified = true;
            }

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

        public static int SellAsset(int id, double sellAmount)
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

            Assets available_money = new Assets();
            available_money.AssetName = "Tiền bán " + asset.AssetName;
            available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
            available_money.CreatedBy = Constants.Constants.USER;
            available_money.CreatedDate = DateTime.Now;
            available_money.Value = sellAmount;
            available_money.Username = asset.Username;

            entities.Assets.Add(available_money);

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.SELL, "tài sản \"" + asset.AssetName + "\"", asset.Username, sellAmount);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }


        public static double CheckAvailableMoney(string username)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }
    }
}