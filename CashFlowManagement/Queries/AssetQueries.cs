using CashFlowManagement.EntityModel;
using CashFlowManagement.Utilities;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
                                                     type == (int)Constants.Constants.ASSET_TYPE.STOCK ?
                                                    (from asset in entities.Assets
                                                     where asset.Username == username && asset.AssetType == type
                                                     && !asset.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset }).ToList()
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

        public static AssetViewModel GetAssetById(int id, int transactionId)
        {
            Entities entities = new Entities();
            var type = entities.Assets.Where(x => x.Id == id).FirstOrDefault().AssetType;
            AssetViewModel result = type == (int)Constants.Constants.ASSET_TYPE.INSURANCE ?
                                                    (from asset in entities.Assets
                                                     join liability in entities.Liabilities on asset.Id equals liability.AssetId
                                                     where asset.Id == id && !asset.DisabledDate.HasValue && !liability.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset, Liability = liability }).FirstOrDefault()
                                                     :
                                                     type == (int)Constants.Constants.ASSET_TYPE.STOCK ?
                                                    (from asset in entities.Assets
                                                     join transaction in entities.StockTransactions on asset.Id equals transaction.AssetId
                                                     where asset.Id == id && transaction.Id == transactionId && !asset.DisabledDate.HasValue
                                                     select new AssetViewModel { Asset = asset, Transaction = transaction }).FirstOrDefault()
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
            else if (type == (int)Constants.Constants.ASSET_TYPE.INSURANCE)
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
            else if (type == (int)Constants.Constants.ASSET_TYPE.STOCK)
            {
                StockTransactions transaction = new StockTransactions
                {
                    Name = "Tạo cổ phiếu " + model.Asset.AssetName,
                    Value = model.Transaction.Value,
                    TransactionDate = model.Transaction.TransactionDate,
                    TransactionType = (int)Constants.Constants.TRANSACTION_TYPE.CREATE,
                    NumberOfShares = model.Transaction.NumberOfShares,
                    SpotPrice = model.Transaction.SpotPrice,
                    BrokerFee = model.Transaction.BrokerFee,
                    ExpectedDividend = model.Transaction.ExpectedDividend,
                    Note = model.Transaction.Note,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Constants.Constants.USER,
                    Username = username
                };


                if (!CheckExistAssetName(model.Asset.AssetName, model.Asset.AssetType))
                {
                    asset.StockTransactions.Add(transaction);
                }
                else
                {
                    Assets current = entities.Assets.Where(x => x.AssetName.Equals(model.Asset.AssetName)
                                                            && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                            && !x.DisabledDate.HasValue).FirstOrDefault();
                    current.StockTransactions.Add(transaction);
                }
            }

            if (type != (int)Constants.Constants.ASSET_TYPE.STOCK && type != (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Incomes income = model.Income;
                income.CreatedDate = DateTime.Now;
                income.IncomeType = type;
                income.Username = username;
                income.CreatedBy = Constants.Constants.USER;
                asset.Incomes.Add(income);
            }


            if (type != (int)Constants.Constants.ASSET_TYPE.STOCK ||
                (type == (int)Constants.Constants.ASSET_TYPE.STOCK && !CheckExistAssetName(model.Asset.AssetName, model.Asset.AssetType)))
            {
                entities.Assets.Add(asset);
            }

            string sType = string.Empty;

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.ADD, "tài sản \"" + model.Asset.AssetName + "\"", username, model.Asset.Value, DateTime.Now);

            entities.Log.Add(log);
            int result = entities.SaveChanges();
            return result;
        }

        public static int BuyAsset(AssetViewModel model, string username)
        {
            Entities entities = new Entities();
            if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE)
            {
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
                asset.AssetType = model.Asset.AssetType;
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

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "bất động sản " + model.Asset.AssetName , username, model.BuyAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.EXPENSE, "tiền mua bất động sản " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);

                entities.Log.Add(log);
                entities.Log.Add(log_2);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK)
            {
                Assets asset = entities.Assets.Where(x => x.AssetName.Equals(model.Asset.AssetName) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                && !x.DisabledDate.HasValue).FirstOrDefault ();
                if (asset == null)
                {
                    asset = new Assets
                    {
                        AssetName = model.Asset.AssetName,
                        AssetType = model.Asset.AssetType,
                        Value = 0,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Constants.Constants.USER,
                        Username = username
                    };
                    entities.Assets.Add(asset);
                }
                StockTransactions transaction = new StockTransactions
                {
                    Name = "Mua cổ phiếu " + model.Asset.AssetName,
                    TransactionDate = model.Transaction.TransactionDate,
                    TransactionType = (int)Constants.Constants.TRANSACTION_TYPE.BUY,
                    NumberOfShares = model.Transaction.NumberOfShares,
                    SpotPrice = model.Transaction.SpotPrice,
                    ExpectedDividend = model.Transaction.ExpectedDividend,
                    Note = model.Transaction.Note,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Constants.Constants.USER,
                    Username = username,
                };
                transaction.BrokerFee = transaction.SpotPrice * transaction.NumberOfShares * 0.0015;
                transaction.Value = transaction.SpotPrice * transaction.NumberOfShares * (1 + 0.0015);
                foreach (var liability in model.Asset.Liabilities)
                {
                    liability.Username = username;
                    liability.CreatedDate = DateTime.Now;
                    liability.CreatedBy = Constants.Constants.USER;
                    liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.STOCK;
                    liability.AssetId = asset.Id > 0 ? asset.Id : (int?) null;
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
                    childLiability.AssetId = asset.Id > 0 ? asset.Id : (int?)null;
                    transaction.Liabilities.Add(liability);
                    transaction.Liabilities.Add(childLiability);
                    asset.Liabilities.Add(liability);
                    asset.Liabilities.Add(childLiability);
                }

                asset.StockTransactions.Add(transaction);

                Assets available_money = new Assets();
                available_money.AssetName = "Tiền mua cổ phiếu" + asset.AssetName;
                available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                available_money.CreatedBy = Constants.Constants.USER;
                available_money.CreatedDate = DateTime.Now;
                available_money.Value = model.BuyAmount * (-1);
                available_money.Username = username;

                entities.Assets.Add(available_money);

                string sType = string.Empty;

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "cổ phiếu " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.EXPENSE, "tiền mua cổ phiếu " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);

                entities.Log.Add(log);
                entities.Log.Add(log_2);
            }
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateAsset(AssetViewModel model)
        {
            Entities entities = new Entities();
            if (model.Asset.AssetType != (int)Constants.Constants.ASSET_TYPE.STOCK &&
                model.Asset.AssetType != (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
                Assets updated_asset = model.Asset;
                updated_asset.CreatedDate = DateTime.Now;
                updated_asset.CreatedBy = Constants.Constants.USER;

                foreach (var liability in asset.Liabilities)
                {
                    updated_asset.Liabilities.Add(liability);
                }

                Incomes updated_income = model.Income;
                updated_income.CreatedDate = DateTime.Now;
                updated_income.CreatedBy = Constants.Constants.USER;
                updated_income.Username = model.Asset.Username;
                updated_asset.Incomes.Add(updated_income);

                asset.DisabledDate = DateTime.Now;
                asset.DisabledBy = Constants.Constants.USER;
                entities.Assets.Attach(asset);
                entities.Entry(asset).State = System.Data.Entity.EntityState.Modified;

                entities.Assets.Add(updated_asset);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.INSURANCE)
            {
                Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
                Assets updated_asset = model.Asset;
                updated_asset.CreatedDate = DateTime.Now;
                updated_asset.CreatedBy = Constants.Constants.USER;

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

                foreach (var lblt in asset.Liabilities)
                {
                    lblt.DisabledDate = DateTime.Now;
                    lblt.DisabledBy = Constants.Constants.USER;
                    entities.Liabilities.Attach(lblt);
                    entities.Entry(lblt).State = System.Data.Entity.EntityState.Modified;
                }

                asset.DisabledDate = DateTime.Now;
                asset.DisabledBy = Constants.Constants.USER;
                entities.Assets.Attach(asset);
                entities.Entry(asset).State = System.Data.Entity.EntityState.Modified;

                updated_asset.Liabilities.Add(liability);
                entities.Assets.Add(updated_asset);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK)
            {
                Assets updated_asset = model.Asset;
                updated_asset.CreatedDate = DateTime.Now;
                updated_asset.CreatedBy = Constants.Constants.USER;

                Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
                StockTransactions transaction = entities.StockTransactions.Where(x => x.Id == model.Transaction.Id).FirstOrDefault();
                transaction.DisabledDate = DateTime.Now;
                transaction.DisabledBy = Constants.Constants.USER;
                entities.StockTransactions.Attach(transaction);
                entities.Entry(transaction).State = System.Data.Entity.EntityState.Modified;

                StockTransactions updated_transaction = new StockTransactions
                {
                    Name = "Cập nhật cổ phiếu " + model.Asset.AssetName,
                    Value = model.Transaction.Value,
                    TransactionDate = model.Transaction.TransactionDate,
                    TransactionType = transaction.TransactionType,
                    NumberOfShares = model.Transaction.NumberOfShares,
                    SpotPrice = model.Transaction.SpotPrice,
                    BrokerFee = model.Transaction.BrokerFee,
                    ExpectedDividend = model.Transaction.ExpectedDividend,
                    Note = model.Transaction.Note,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Constants.Constants.USER,
                    Username = model.Asset.Username
                };

                foreach (var liability in transaction.Liabilities)
                {
                    updated_transaction.Liabilities.Add(liability);
                }
                asset.StockTransactions.Add(updated_transaction); 

                if(transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY || transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL)
                {
                    double changedAmount = transaction.Value - updated_transaction.Value;

                    Assets available_money = new Assets();
                    available_money.AssetName = "Tiền thay đổi cập nhật cổ phiếu " + asset.AssetName;
                    available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                    available_money.CreatedBy = Constants.Constants.USER;
                    available_money.CreatedDate = DateTime.Now;
                    if (transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY)
                    {
                        available_money.Value = changedAmount;
                    }
                    else if (transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL)
                    {
                        available_money.Value = 0 - changedAmount;
                    }
                    available_money.Username = asset.Username;

                    entities.Assets.Add(available_money);

                    var logs = entities.Log.Where(x => DbFunctions.TruncateTime(x.Date) >= updated_transaction.TransactionDate && x.Username.Equals(asset.Username));
                    foreach (var item in logs)
                    {
                        item.AvailableMoney += available_money.Value;
                        entities.Log.Attach(item);
                        entities.Entry(item).State = EntityState.Modified;

                    }
                }
            }
            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.UPDATE, "tài sản \"" + model.Asset.AssetName + "\"", model.Asset.Username, model.Asset.Value, DateTime.Now);
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

            if (asset.AssetType != (int)Constants.Constants.ASSET_TYPE.INSURANCE && asset.AssetType != (int)Constants.Constants.ASSET_TYPE.STOCK)
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

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.DELETE, "tài sản \"" + asset.AssetName + "\"", asset.Username, asset.Value, DateTime.Now);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }

        public static int SellAsset(AssetViewModel model)
        {
            Entities entities = new Entities();
            if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE)
            {
                Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
                asset.DisabledDate = DateTime.Now;
                asset.DisabledBy = Constants.Constants.USER;

                entities.Assets.Attach(asset);
                var entry = entities.Entry(asset);
                entry.Property(x => x.DisabledDate).IsModified = true;
                entry.Property(x => x.DisabledBy).IsModified = true;

                Incomes income = entities.Incomes.Where(x => x.AssetId == model.Asset.Id && !x.DisabledDate.HasValue).FirstOrDefault();
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
                available_money.Value = model.SellAmount;
                available_money.Username = asset.Username;

                entities.Assets.Add(available_money);

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.SELL, "bất động sản " + asset.AssetName, asset.Username, model.SellAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, "tiền bán bất động sản " + asset.AssetName, asset.Username, model.SellAmount, DateTime.Now);
                entities.Log.Add(log);
                entities.Log.Add(log_2);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK)
            {
                Assets asset = entities.Assets.Where(x => x.AssetName.Equals(model.Asset.AssetName) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                && !x.DisabledDate.HasValue).FirstOrDefault();
                if (asset != null)
                {
                    int currentNumberOfShare = entities.StockTransactions.Where(x => x.AssetId == asset.Id).Sum(x => x.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL ? x.NumberOfShares * -1 : x.NumberOfShares);
                    if (currentNumberOfShare >= model.Transaction.NumberOfShares)
                    {
                        StockTransactions transaction = new StockTransactions
                        {
                            Name = "Bán cổ phiếu " + model.Asset.AssetName,
                            Value = model.Transaction.NumberOfShares * model.Transaction.SpotPrice * (1 + 0.0025),
                            TransactionDate = DateTime.Now,
                            TransactionType = (int)Constants.Constants.TRANSACTION_TYPE.SELL,
                            NumberOfShares = model.Transaction.NumberOfShares,
                            SpotPrice = model.Transaction.SpotPrice,
                            BrokerFee = model.Transaction.NumberOfShares * model.Transaction.SpotPrice * 0.0025,
                            ExpectedDividend = model.Transaction.ExpectedDividend,
                            Note = model.Transaction.Note,
                            CreatedDate = DateTime.Now,
                            CreatedBy = Constants.Constants.USER,
                            Username = asset.Username
                        };

                        asset.StockTransactions.Add(transaction);

                        Assets available_money = new Assets();
                        available_money.AssetName = "Tiền bán cổ phiếu " + asset.AssetName;
                        available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                        available_money.CreatedBy = Constants.Constants.USER;
                        available_money.CreatedDate = DateTime.Now;
                        available_money.Value = model.Transaction.NumberOfShares * model.Transaction.SpotPrice * (1 + 0.0025);
                        available_money.Username = asset.Username;

                        entities.Assets.Add(available_money);

                        Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.SELL, "cổ phiếu " + asset.AssetName , asset.Username, transaction.Value, DateTime.Now);
                        Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, "tiền bán cổ phiếu " + asset.AssetName, asset.Username, transaction.Value, DateTime.Now);
                        entities.Log.Add(log);
                        entities.Log.Add(log_2);
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -1;
                }
            }
            int result = entities.SaveChanges();
            return result;
        }

        public static double CheckAvailableMoney(string username, DateTime date)
        {
            Entities entities = new Entities();
            var dataDate = date.Date;
            return entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                         && !x.DisabledDate.HasValue && DbFunctions.TruncateTime(x.CreatedDate) <= dataDate).Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public static bool CheckExistAssetName(string assetName, int type)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.AssetName.Equals(assetName) && x.AssetType == type).Any();
        }

        public static double CheckRemainedStock(string stock)
        {
            Entities entities = new Entities();
            Assets asset = entities.Assets.Where(x => x.AssetName.Equals(stock)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                         && !x.DisabledDate.HasValue).FirstOrDefault();
            if (asset != null)
            {
                return entities.StockTransactions.Where(x => x.AssetId.Equals(asset.Id))
                    .Select(x => x.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL ? x.NumberOfShares * -1 : x.NumberOfShares).DefaultIfEmpty(0).Sum();
            }
            else
            {
                return -1;
            }
        }

        public static void CreateCashFlowPerMonth()
        {
            Entities entities = new Entities();
            var users = entities.Users;
            foreach (var user in users)
            {
                double salaryIncome = entities.Incomes.Where(x => x.Username.Equals(user.Username)
                                                         && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double realEstateIncome = entities.Incomes.Where(x => x.Username.Equals(user.Username)
                                                             && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.REAL_ESTATE_INCOME
                                                             && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double businessIncome = entities.Incomes.Where(x => x.Username.Equals(user.Username)
                                                             && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.BUSINESS_INCOME
                                                             && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double interestIncome = entities.Incomes.Where(x => x.Username.Equals(user.Username)
                                                             && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.BANK_DEPOSIT_INCOME
                                                             && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double stockIncome = 0;

                double familyExpenses = entities.Expenses.Where(x => x.Username.Equals(user.Username)
                                                             && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.FAMILY
                                                             && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double otherExpenses = entities.Expenses.Where(x => x.Username.Equals(user.Username)
                                                             && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.OTHERS
                                                             && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

                double carExpenses = 0, creditCardExpenses = 0, homeMortgage = 0, businessLoanExpenses = 0, otherLoanExpenses = 0, stockExpenses = 0;

                var carLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                         && !x.DisabledDate.HasValue);
                foreach (var carLiability in carLiabilities)
                {
                    carExpenses += carLiability.Value / FormatUtility.CalculateTimePeriod(carLiability.StartDate.Value, carLiability.EndDate.Value) + carLiability.Value * carLiability.InterestRate / 1200;
                }

                var creditCardLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                             && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                             && !x.DisabledDate.HasValue);
                foreach (var creditCarLiability in creditCardLiabilities)
                {
                    creditCardExpenses += creditCarLiability.Value / 12 + creditCarLiability.Value * creditCarLiability.InterestRate / 1200;
                }

                var homeLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE
                                                         && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);

                foreach (var homeLiability in homeLiabilities)
                {
                    homeMortgage += LiabilityQueries.GetCurrentMonthlyPayment(homeLiability.Id);
                }

                var businessLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                             && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS
                                                             && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
                foreach (var businessLiability in businessLiabilities)
                {
                    businessLoanExpenses += LiabilityQueries.GetCurrentMonthlyPayment(businessLiability.Id);
                }

                var otherLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                             && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                             && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
                foreach (var otherLiability in otherLiabilities)
                {
                    otherLoanExpenses  += otherLiability.Value / FormatUtility.CalculateTimePeriod(otherLiability.StartDate.Value, otherLiability.EndDate.Value) + otherLiability.Value * otherLiability.InterestRate / 1200;
                }

                var insuranceExpenses = entities.Assets.Where(x => x.Username.Equals(user.Username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.INSURANCE
                                                         && !x.DisabledDate.HasValue).Sum(x => x.Liabilities.Sum(y => y.Value));

                double totalIncomes = salaryIncome + realEstateIncome + businessIncome + interestIncome + stockIncome;
                double totalExpenses = homeMortgage + carExpenses + creditCardExpenses + businessLoanExpenses + stockExpenses + otherExpenses + familyExpenses + insuranceExpenses + otherExpenses;
                double monthlyCashflow = totalIncomes - totalExpenses;

                var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                    && x.AssetName.Equals("CashFlow:" + DateTime.Now.ToString("MM/yyyy")));
                if(!cf.Any())
                {
                    Assets cashflow = new Assets();
                    cashflow.AssetName = "CashFlow:" + DateTime.Now.ToString("MM/yyyy");
                    cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                    cashflow.CreatedDate = DateTime.Now;
                    cashflow.CreatedBy = Constants.Constants.USER;
                    cashflow.Username = user.Username;
                    cashflow.Value = monthlyCashflow;

                    entities.Assets.Add(cashflow);

                    Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, DateTime.Now);

                    entities.Log.Add(log);
                    int result = entities.SaveChanges();
                }
            }
        }
    }
}