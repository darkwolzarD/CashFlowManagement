﻿using CashFlowManagement.EntityModel;
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
                                                     && !asset.DisabledDate.HasValue && asset.StockTransactions.Where(x => !x.DisabledDate.HasValue).Count() > 0
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
            if (type == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY)
            {
                if (model.Asset.ObtainedBy == (int)Constants.Constants.OBTAIN_BY.EXPENSE)
                {
                    model.Asset.Value = 0 - model.Asset.Value;
                }
            }

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


                if (!CheckExistAssetName(model.Asset.AssetName, username, model.Asset.AssetType))
                {
                    asset.StockTransactions.Add(transaction);
                }
                else
                {
                    Assets current = entities.Assets.Where(x => x.AssetName.Equals(model.Asset.AssetName)
                                                            && x.Username.Equals(username)
                                                            && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                            && !x.DisabledDate.HasValue).FirstOrDefault();
                    transaction.AssetId = current.Id;
                }
                entities.StockTransactions.Add(transaction);
            }



            if (type != (int)Constants.Constants.ASSET_TYPE.STOCK && type != (int)Constants.Constants.ASSET_TYPE.INSURANCE
                && type != (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY)
            {
                Incomes income = model.Income;
                if (type == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
                {
                    income = new Incomes();
                    income.Value = asset.Value * asset.InterestRate.Value / 1200;
                    income.StartDate = asset.StartDate.Value;
                    income.EndDate = asset.EndDate.Value;
                }
                income.CreatedDate = DateTime.Now;
                income.IncomeType = type;
                income.Username = username;
                income.CreatedBy = Constants.Constants.USER;
                asset.Incomes.Add(income);
            }


            if (type != (int)Constants.Constants.ASSET_TYPE.STOCK ||
                (type == (int)Constants.Constants.ASSET_TYPE.STOCK && !CheckExistAssetName(model.Asset.AssetName, username, model.Asset.AssetType)))
            {
                entities.Assets.Add(asset);
            }

            string sType = string.Empty;

            Log log = new Log();
            if (asset.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY)
            {
                log = LogQueries.CreateLog(asset.ObtainedBy == (int)Constants.Constants.OBTAIN_BY.INCOME ? (int)Constants.Constants.LOG_TYPE.INCOME : (int)Constants.Constants.LOG_TYPE.EXPENSE, model.Asset.AssetName, username, model.Asset.Value, model.Asset.StartDate.Value);
            }
            else
            {
                log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.ADD, "tài sản " + model.Asset.AssetName, username, model.Asset.Value, DateTime.Now);
            }
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

                asset.Assets2 = available_money;

                entities.Assets.Add(asset);
                entities.Assets.Add(available_money);

                string sType = string.Empty;

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "bất động sản " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.EXPENSE, "tiền mua bất động sản " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);

                entities.Log.Add(log);
                entities.Log.Add(log_2);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK)
            {
                Assets asset = entities.Assets.Where(x => x.AssetName.Equals(model.Asset.AssetName) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                && !x.DisabledDate.HasValue).FirstOrDefault();
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
                    liability.AssetId = asset.Id > 0 ? asset.Id : (int?)null;
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

                Assets available_money = new Assets();
                available_money.AssetName = "Tiền mua cổ phiếu " + asset.AssetName;
                available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                available_money.CreatedBy = Constants.Constants.USER;
                available_money.CreatedDate = DateTime.Now;
                available_money.Value = model.BuyAmount * (-1);
                available_money.Username = username;

                transaction.Assets1 = available_money;

                asset.StockTransactions.Add(transaction);
                entities.Assets.Add(available_money);

                string sType = string.Empty;

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "cổ phiếu " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.EXPENSE, "tiền mua cổ phiếu " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);

                entities.Log.Add(log);
                entities.Log.Add(log_2);
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
            {
                Assets asset = model.Asset;
                asset.Username = username;
                asset.CreatedDate = DateTime.Now;
                asset.AssetType = model.Asset.AssetType;
                asset.CreatedBy = Constants.Constants.USER;
                asset.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.BUY;

                Incomes income = new Incomes();
                income.CreatedBy = Constants.Constants.USER;
                income.CreatedDate = DateTime.Now;
                income.EndDate = asset.EndDate;
                income.IncomeType = (int)Constants.Constants.INCOME_TYPE.BANK_DEPOSIT_INCOME;
                income.Note = income.Note;
                income.StartDate = asset.StartDate.Value;
                income.Username = asset.Username;
                income.Value = asset.Value * asset.InterestRate.Value / 1200;
                asset.Incomes.Add(income);

                Assets available_money = new Assets();
                available_money.AssetName = "Tiền tạo tài khoản tiết kiệm " + model.Asset.AssetName;
                available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                available_money.CreatedBy = Constants.Constants.USER;
                available_money.CreatedDate = DateTime.Now;
                available_money.Value = 0 - asset.Value;
                available_money.Username = username;

                asset.Assets2 = available_money;

                asset.Incomes.Add(income);
                entities.Assets.Add(asset);
                entities.Assets.Add(available_money);

                string sType = string.Empty;

                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.BUY, "tài khoản tiết kiệm " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);
                Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.EXPENSE, "tiền tạo tài khoản tiết kiệm " + model.Asset.AssetName, username, model.BuyAmount, DateTime.Now);

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

                foreach (var income in asset.Incomes)
                {
                    income.DisabledDate = DateTime.Now;
                    income.DisabledBy = Constants.Constants.USER;
                    entities.Incomes.Attach(income);
                    entities.Entry(income).State = EntityState.Modified;
                }

                if (asset.CashId != null)
                {
                    Assets buyMoney = entities.Assets.Where(x => x.Id == asset.CashId).FirstOrDefault();
                    buyMoney.DisabledDate = DateTime.Now;
                    buyMoney.DisabledBy = Constants.Constants.USER;
                    entities.Assets.Attach(buyMoney);
                    entities.Entry(buyMoney).State = EntityState.Modified;

                    Assets available_money = new Assets();
                    available_money.AssetName = "Tiền tạo tài khoản tiết kiệm " + updated_asset.AssetName;
                    available_money.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                    available_money.CreatedBy = Constants.Constants.USER;
                    available_money.CreatedDate = DateTime.Now;
                    available_money.Value = 0 - updated_asset.Value;
                    available_money.Username = updated_asset.Username;

                    updated_asset.Assets2 = available_money;
                    entities.Assets.Add(available_money);
                }

                Incomes updated_income = model.Income;
                if (asset.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
                {
                    updated_income = new Incomes();
                    updated_income.Value = model.Asset.Value * model.Asset.InterestRate.Value / 1200;
                    updated_income.StartDate = model.Asset.StartDate.Value;
                    updated_income.EndDate = model.Asset.EndDate.Value;
                }
                updated_income.IncomeType = asset.Incomes.FirstOrDefault().IncomeType;
                updated_income.CreatedDate = DateTime.Now;
                updated_income.CreatedBy = Constants.Constants.USER;
                updated_income.Username = model.Asset.Username;
                updated_asset.Incomes.Add(updated_income);

                asset.DisabledDate = DateTime.Now;
                asset.DisabledBy = Constants.Constants.USER;
                entities.Assets.Attach(asset);
                entities.Entry(asset).State = EntityState.Modified;

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
                    SpotPrice = model.Transaction.NumberOfShares,
                    BrokerFee = model.Transaction.NumberOfShares * model.Transaction.NumberOfShares * 0.0015,
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

                Assets cash = entities.Assets.Where(x => x.Username.Equals(asset.Username) && x.Id == transaction.CashId).FirstOrDefault();
                if (cash != null)
                {
                    cash.DisabledDate = DateTime.Now;
                    cash.DisabledBy = Constants.Constants.SYSTEM;
                    entities.Assets.Attach(cash);
                    entities.Entry(cash).State = EntityState.Modified;
                }

                asset.StockTransactions.Add(updated_transaction);

                if (transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY || transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL)
                {
                    double changedAmount = transaction.Assets1.Value - transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY ? model.Transaction.Assets1.Value * -1 : model.Transaction.Assets1.Value;

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

                    Assets available_money_2 = new Assets();
                    available_money_2.AssetName = transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL ? "Tiền mua cổ phiếu " : "Tiền bán cổ phiếu " + asset.AssetName;
                    available_money_2.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                    available_money_2.CreatedBy = Constants.Constants.USER;
                    available_money_2.CreatedDate = DateTime.Now;
                    available_money_2.Value = transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.SELL ? model.Transaction.Assets1.Value : model.Transaction.Assets1.Value * -1;
                    available_money_2.Username = asset.Username;

                    updated_transaction.Assets1 = available_money_2;

                    entities.Assets.Add(available_money);
                    entities.Assets.Add(available_money_2);

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

        public static int DeleteAsset(int id, int transactionId)
        {
            Entities entities = new Entities();
            if (transactionId == 0)
            {
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
            }
            else
            {
                StockTransactions transaction = entities.StockTransactions.Where(x => x.Id == transactionId).FirstOrDefault();
                transaction.DisabledDate = DateTime.Now;
                transaction.DisabledBy = Constants.Constants.USER;
                entities.StockTransactions.Attach(transaction);
                entities.Entry(transaction).State = EntityState.Modified;
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static int SellAsset(AssetViewModel model)
        {
            Entities entities = new Entities();
            if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
            {
                Assets asset = entities.Assets.Where(x => x.Id == model.Asset.Id).FirstOrDefault();
                if (model.Asset.Value == 0)
                {
                    return -1;
                }
                else
                {
                    if (asset.Value < model.Asset.Value)
                    {
                        return -2;
                    }
                    else if (asset.Value > model.Asset.Value)
                    {
                        asset.DisabledDate = DateTime.Now;
                        asset.DisabledBy = Constants.Constants.USER;
                        entities.Assets.Attach(asset);
                        entities.Entry(asset).State = EntityState.Modified;

                        Assets new_bank_deposit = new Assets();
                        new_bank_deposit.AssetName = asset.AssetName;
                        new_bank_deposit.AssetType = (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT;
                        new_bank_deposit.CreatedBy = Constants.Constants.USER;
                        new_bank_deposit.CreatedDate = DateTime.Now;
                        new_bank_deposit.EndDate = asset.EndDate;
                        new_bank_deposit.InterestRate = asset.InterestRate;
                        new_bank_deposit.Note = asset.Note;
                        new_bank_deposit.ObtainedBy = asset.ObtainedBy;
                        new_bank_deposit.StartDate = asset.StartDate;
                        new_bank_deposit.Term = asset.Term;
                        new_bank_deposit.Username = asset.Username;
                        new_bank_deposit.Value = asset.Value - model.Asset.Value;

                        if (asset.ObtainedBy == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.ORIGIN)
                        {

                        }
                        else if (asset.ObtainedBy == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.START)
                        {

                        }
                        else if (asset.ObtainedBy == (int)Constants.Constants.INTEREST_OBTAIN_TYPE.END)
                        {
                            Incomes income = entities.Incomes.Where(x => x.AssetId == asset.Id && !x.DisabledDate.HasValue).FirstOrDefault();
                            income.DisabledDate = DateTime.Now;
                            income.DisabledBy = Constants.Constants.USER;
                            entities.Incomes.Attach(income);
                            entities.Entry(income).State = EntityState.Modified;

                            Incomes new_income = new Incomes();
                            new_income.CreatedBy = Constants.Constants.USER;
                            new_income.CreatedDate = DateTime.Now;
                            new_income.EndDate = new_bank_deposit.EndDate;
                            new_income.IncomeType = income.IncomeType;
                            new_income.Note = income.Note;
                            new_income.StartDate = new_bank_deposit.StartDate.Value;
                            new_income.Username = new_bank_deposit.Username;
                            new_income.Value = new_bank_deposit.Value * new_bank_deposit.InterestRate.Value / 1200;

                            Assets availableMoney = new Assets();
                            availableMoney.AssetName = "Tiền rút tiết kiệm " + asset.AssetName;
                            availableMoney.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            availableMoney.CreatedDate = DateTime.Now;
                            availableMoney.CreatedBy = Constants.Constants.USER;
                            availableMoney.Username = asset.Username;
                            availableMoney.Value = model.Asset.Value;

                            asset.Assets2 = availableMoney;      //cashID do ban tai san//
                            entities.Assets.Add(availableMoney);

                            new_bank_deposit.Incomes.Add(new_income);
                            entities.Incomes.Add(new_income);
                            entities.Assets.Add(new_bank_deposit);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.WITHDRAW, "tiền tiết kiệm " + asset.AssetName, asset.Username, model.Asset.Value, DateTime.Now);
                            Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, "tiền rút tiết kiệm " + asset.AssetName, asset.Username, model.Asset.Value, DateTime.Now);
                            entities.Log.Add(log);
                            entities.Log.Add(log_2);
                        }
                    }
                    else    //(asset.Value == model.Asset.Value)//                               
                    {
                        asset.DisabledDate = DateTime.Now;
                        asset.DisabledBy = Constants.Constants.USER;
                        entities.Assets.Attach(asset);
                        entities.Entry(asset).State = EntityState.Modified;

                        Incomes income = entities.Incomes.Where(x => x.AssetId == asset.Id && !x.DisabledDate.HasValue).FirstOrDefault();
                        income.DisabledDate = DateTime.Now;
                        income.DisabledBy = Constants.Constants.USER;
                        entities.Incomes.Attach(income);
                        entities.Entry(income).State = EntityState.Modified;

                        Assets availableMoney = new Assets();
                        availableMoney.AssetName = "Tiền rút tiết kiệm " + asset.AssetName;
                        availableMoney.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                        availableMoney.CreatedDate = DateTime.Now;
                        availableMoney.CreatedBy = Constants.Constants.USER;
                        availableMoney.Username = asset.Username;

                        double non_term_interest_rate = 0;
                        double non_term_interest_money = model.Asset.Value * non_term_interest_rate;

                        availableMoney.Value = model.Asset.Value + non_term_interest_money;

                        asset.Assets2 = availableMoney;

                        Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.WITHDRAW, "tiền tiết kiệm " + asset.AssetName, asset.Username, model.Asset.Value, DateTime.Now);
                        Log log_2 = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, "tiền rút tiết kiệm " + asset.AssetName, asset.Username, model.Asset.Value, DateTime.Now);
                        entities.Log.Add(log);
                        entities.Log.Add(log_2);
                    }
                }
            }
            else if (model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE)
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

                        transaction.Assets1 = available_money;
                        entities.Assets.Add(available_money);

                        Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.SELL, "cổ phiếu " + asset.AssetName, asset.Username, transaction.Value, DateTime.Now);
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

        public static bool CheckExistAssetName(string assetName, string username, int type)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.AssetName.Equals(assetName) && x.Username.Equals(username) && x.AssetType == type && !x.DisabledDate.HasValue).Any();
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
            var current = DateTime.Now;
            foreach (var user in users)
            {
                var incomes = entities.Incomes.Where(x => x.Username.Equals(user.Username)
                                                         && !x.DisabledDate.HasValue);

                foreach (var income in incomes)
                {
                    if (income.IncomeDay.HasValue ? income.IncomeDay == current.Day : current.Day == 1)
                    {
                        var assetName = "CashFlow:" + (string.IsNullOrEmpty(income.Name) ? income.Assets.AssetName : income.Name) + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + (string.IsNullOrEmpty(income.Name) ? income.Assets.AssetName : income.Name) + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = income.Value;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }
                }

                if (current.Day == 1)
                {
                    var stocks = entities.Assets.Where(x => x.Username.Equals(user.Username)
                                                             && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                             && !x.DisabledDate.HasValue);

                    foreach (var stock in stocks)
                    {
                        var transactions = entities.StockTransactions.Where(x => x.Username.Equals(user.Username) && x.AssetId == stock.Id && !x.DisabledDate.HasValue);
                        if (transactions.Any())
                        {
                            double stockIncome = 0;
                            int numberOfShares = entities.StockTransactions.Where(x => x.Username.Equals(user.Username) && x.AssetId == stock.Id && !x.DisabledDate.HasValue).Sum(x => x.NumberOfShares);
                            double currentPrice = entities.StockTransactions.Where(x => x.Username.Equals(user.Username) && x.AssetId == stock.Id && !x.DisabledDate.HasValue).OrderByDescending(x => x.TransactionDate).FirstOrDefault().SpotPrice;
                            double interestRate = entities.StockTransactions.Where(x => x.Username.Equals(user.Username) && x.AssetId == stock.Id && !x.DisabledDate.HasValue).OrderByDescending(x => x.TransactionDate).FirstOrDefault().ExpectedDividend;
                            stockIncome += numberOfShares * currentPrice * interestRate / 100;

                            var assetName = "CashFlow:" + stock.AssetName + "(" + current.ToString("MM/yyyy") + ")";
                            var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                            if (!cf.Any())
                            {
                                Assets cashflow = new Assets();
                                cashflow.AssetName = "CashFlow:" + stock.AssetName + "(" + current.ToString("MM/yyyy") + ")";
                                cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                                cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                                cashflow.CreatedDate = current;
                                cashflow.CreatedBy = Constants.Constants.USER;
                                cashflow.Username = user.Username;
                                cashflow.Value = stockIncome;

                                entities.Assets.Add(cashflow);

                                Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                                entities.Log.Add(log);
                            }
                        }
                    }
                }

                var expenses = entities.Expenses.Where(x => x.Username.Equals(user.Username)
                                                             && !x.DisabledDate.HasValue);

                foreach (var expense in expenses)
                {
                    if (expense.ExpenseDay == current.Day)
                    {
                        var assetName = "CashFlow:" + expense.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + expense.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - expense.Value;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }
                }

                if (current.Day == 1)
                {
                    var carLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                             && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                             && !x.DisabledDate.HasValue);
                    foreach (var carLiability in carLiabilities)
                    {
                        double carExpense = carLiability.Value / FormatUtility.CalculateTimePeriod(carLiability.StartDate.Value, carLiability.EndDate.Value) + carLiability.Value * carLiability.InterestRate / 1200;
                        var assetName = "CashFlow:" + carLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + carLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - carExpense;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }

                    var creditCardLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                                 && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                                 && !x.DisabledDate.HasValue);
                    foreach (var creditCarLiability in creditCardLiabilities)
                    {
                        double creditCardExpense = creditCarLiability.Value / 12 + creditCarLiability.Value * creditCarLiability.InterestRate / 1200;
                        var assetName = "CashFlow:" + creditCarLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + creditCarLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - creditCardExpense;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }

                    var homeLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                             && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE
                                                             && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);

                    foreach (var homeLiability in homeLiabilities)
                    {
                        double homeMortgage = LiabilityQueries.GetCurrentMonthlyPayment(homeLiability.Id);

                        var assetName = "CashFlow:" + homeLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + homeLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - homeMortgage;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }

                    var businessLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                                 && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS
                                                                 && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
                    foreach (var businessLiability in businessLiabilities)
                    {
                        double businessLoanExpense = LiabilityQueries.GetCurrentMonthlyPayment(businessLiability.Id);
                        var assetName = "CashFlow:" + businessLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + businessLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - businessLoanExpense;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }

                    var otherLiabilities = entities.Liabilities.Where(x => x.Username.Equals(user.Username)
                                                                 && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                                 && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
                    foreach (var otherLiability in otherLiabilities)
                    {
                        double otherLoanExpense = otherLiability.Value / FormatUtility.CalculateTimePeriod(otherLiability.StartDate.Value, otherLiability.EndDate.Value) + otherLiability.Value * otherLiability.InterestRate / 1200;
                        var assetName = "CashFlow:" + otherLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + otherLiability.Name + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - otherLoanExpense;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }

                    var insurances = entities.Assets.Where(x => x.Username.Equals(user.Username)
                                                             && x.AssetType == (int)Constants.Constants.ASSET_TYPE.INSURANCE
                                                             && !x.DisabledDate.HasValue);
                    foreach (var insurance in insurances)
                    {
                        double insuranceExpense = insurance.Liabilities.Select(x => x.Value).DefaultIfEmpty(0).Sum();
                        var assetName = "CashFlow:" + insurance.AssetName + "(" + current.ToString("MM/yyyy") + ")";
                        var cf = entities.Assets.Where(x => x.Username.Equals(user.Username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                            && x.AssetName.Equals(assetName));
                        if (!cf.Any())
                        {
                            Assets cashflow = new Assets();
                            cashflow.AssetName = "CashFlow:" + insurance.AssetName + "(" + current.ToString("MM/yyyy") + ")";
                            cashflow.StartDate = new DateTime(current.Year, current.Month, 1);
                            cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                            cashflow.CreatedDate = current;
                            cashflow.CreatedBy = Constants.Constants.USER;
                            cashflow.Username = user.Username;
                            cashflow.Value = 0 - insuranceExpense;

                            entities.Assets.Add(cashflow);

                            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.INCOME, cashflow.AssetName, user.Username, cashflow.Value, current);

                            entities.Log.Add(log);
                        }
                    }
                }
            }
            int result = entities.SaveChanges();
        }

        public static void BankDepositMaturity()
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now.Date;
            var bankDeposits = entities.Assets.Where(x => x.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT
                                                    && !x.DisabledDate.HasValue);
            foreach (var bankDeposit in bankDeposits)
            {
                if (bankDeposit.EndDate.Value.Date.Equals(current))
                {
                    bankDeposit.DisabledDate = DateTime.Now;
                    bankDeposit.DisabledBy = Constants.Constants.SYSTEM;
                    entities.Assets.Attach(bankDeposit);
                    entities.Entry(bankDeposit).State = EntityState.Modified;

                    Assets new_deposit = new Assets();
                    new_deposit.AssetName = bankDeposit.AssetName;
                    new_deposit.AssetType = bankDeposit.AssetType;
                    new_deposit.CreatedBy = Constants.Constants.SYSTEM;
                    new_deposit.CreatedDate = DateTime.Now;
                    new_deposit.StartDate = bankDeposit.EndDate;
                    new_deposit.EndDate = new_deposit.StartDate.Value.AddMonths(bankDeposit.Term.Value);
                    new_deposit.InterestRate = bankDeposit.InterestRate;
                    new_deposit.Note = bankDeposit.Note;
                    new_deposit.ObtainedBy = bankDeposit.ObtainedBy;
                    new_deposit.Term = bankDeposit.Term;
                    new_deposit.CashId = bankDeposit.CashId;
                    new_deposit.Username = bankDeposit.Username;
                    new_deposit.Value = bankDeposit.Value * (1 + bankDeposit.Term.Value * bankDeposit.InterestRate.Value / 1200);

                    Incomes income = entities.Incomes.Where(x => x.AssetId == bankDeposit.Id && !x.DisabledDate.HasValue).FirstOrDefault();
                    income.DisabledDate = DateTime.Now;
                    income.DisabledBy = Constants.Constants.SYSTEM;
                    entities.Incomes.Attach(income);
                    entities.Entry(income).State = EntityState.Modified;

                    Incomes new_income = new Incomes();
                    new_income.CreatedBy = Constants.Constants.SYSTEM;
                    new_income.CreatedDate = DateTime.Now;
                    new_income.EndDate = new_deposit.EndDate;
                    new_income.IncomeType = (int)Constants.Constants.INCOME_TYPE.BANK_DEPOSIT_INCOME;
                    new_income.StartDate = new_deposit.StartDate.Value;
                    new_income.Username = new_deposit.Username;
                    new_income.Value = new_deposit.Value * new_deposit.InterestRate.Value / 1200;

                    new_deposit.Incomes.Add(new_income);

                    entities.Assets.Add(new_deposit);
                    entities.Incomes.Add(new_income);
                }
            }

            entities.SaveChanges();
        }
    }
}