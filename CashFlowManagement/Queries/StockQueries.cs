using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class StockQueries
    {
        public static StockTransactionViewModel CreateViewModel(StockTransactions transaction)
        {
            Entities entities = new Entities();
            StockTransactionViewModel model = new StockTransactionViewModel();
            model.NumberOfStock = transaction.NumberOfShares;
            model.SpotRice = transaction.SpotPrice;
            model.StockValue = transaction.Value;
            model.ExpectedDividend = transaction.ExpectedDividend / 100;

            var liabilities = entities.Liabilities.Where(x => x.TransactionId == transaction.Id);
            foreach (var liability in liabilities.Where(x => !x.DisabledDate.HasValue))
            {
                StockLiabilityViewModel liabilityViewModel = StockLiabilityQueries.CreateViewModel(liability);
                model.Liabilities.Liabilities.Add(liabilityViewModel);
            }

            return model;
        }

        public static bool CheckExistStock(string username, string stockName)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username) && x.AssetName.Equals(stockName)
                                        && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                        && !x.DisabledDate.HasValue).Any();
        }

        public static StockListViewModel GetStockByUser(string username)
        {
            Entities entities = new Entities();
            StockListViewModel result = new StockListViewModel();
            DateTime current = DateTime.Now;

            var stocks = entities.Assets.Include("StockTransactions").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                      && !x.DisabledDate.HasValue);

            foreach (var stock in stocks)
            {
                StockViewModel stockViewModel = new StockViewModel();
                stockViewModel.Id = stock.Id;
                stockViewModel.Name = stock.AssetName;
                stockViewModel.Note = stock.Note;

                foreach (var transactions in stock.StockTransactions.Where(x => !x.DisabledDate.HasValue))
                {
                    StockTransactionViewModel transactionViewModel = StockQueries.CreateViewModel(transactions);
                    stockViewModel.Transactions.Transactions.Add(transactionViewModel);
                }

                var liabilities = stockViewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities.Where(y => y.StartDate <= current && y.EndDate >= current));
                stockViewModel.TotalLiabilityValue = liabilities.Sum(x => x.Sum(y => y.Value.HasValue ? y.Value.Value : 0));
                stockViewModel.TotalOriginalPayment = liabilities.Sum(x => x.Sum(y => y.MonthlyOriginalPayment));
                stockViewModel.TotalInterestPayment = liabilities.Sum(x => x.Sum(y => y.MonthlyInterestPayment));
                stockViewModel.TotalMonthlyPayment = liabilities.Sum(x => x.Sum(y => y.TotalMonthlyPayment));
                stockViewModel.TotalPayment = liabilities.Sum(x => x.Sum(y => y.TotalPayment));
                stockViewModel.TotalRemainedValue = liabilities.Sum(x => x.Sum(y => y.RemainedValue));
                stockViewModel.TotalOriginalInterestPayment = liabilities.Sum(x => x.Sum(y => y.OriginalInterestPayment));
                stockViewModel.TotalInterestRate = stockViewModel.TotalOriginalInterestPayment / stockViewModel.TotalLiabilityValue * 12;
                stockViewModel.RowSpan = stockViewModel.Transactions.Transactions.Any() ? stockViewModel.Transactions.Transactions.Count() + stockViewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Count() + 4 : 4;

                if(stockViewModel.Transactions.Transactions.Any())
                {
                    stockViewModel.RowSpan = 4;
                    bool flag = false;
                    foreach (var transaction in stockViewModel.Transactions.Transactions)
                    {
                        if(transaction.Liabilities.Liabilities.Count() > 0)
                        {
                            if (flag == false)
                            {
                                flag = true;
                            }
                            stockViewModel.RowSpan += transaction.Liabilities.Liabilities.Count();
                        }
                    }
                    if(flag == true)
                    {
                        stockViewModel.RowSpan += 1;
                    }
                }
                else
                {
                    stockViewModel.RowSpan = 4;
                }

                result.Stocks.Add(stockViewModel);
            }

            result.TotalValue = result.Stocks.Select(x => x.Transactions.Transactions).Sum(x => x.Sum(y => y.StockValue.Value));
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static StockSummaryListViewModel GetStockSummaryByUser(string username)
        {
            Entities entities = new Entities();
            StockSummaryListViewModel result = new StockSummaryListViewModel();
            DateTime current = DateTime.Now;

            var stocks = entities.Assets.Include("StockTransactions").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
                                                      && !x.DisabledDate.HasValue);

            foreach (var stock in stocks)
            {
                StockSummaryViewModel stockViewModel = new StockSummaryViewModel();
                stockViewModel.Name = stock.AssetName;
                stockViewModel.Note = stock.Note;

                foreach (var transactions in stock.StockTransactions.Where(x => !x.DisabledDate.HasValue))
                {
                    StockTransactionViewModel transactionViewModel = StockQueries.CreateViewModel(transactions);
                    stockViewModel.NumberOfStock += (int)transactionViewModel.NumberOfStock.Value;
                    stockViewModel.SpotRice += transactionViewModel.SpotRice.Value;
                    stockViewModel.StockValue += transactionViewModel.StockValue.Value;
                    stockViewModel.ExpectedDividend += transactionViewModel.ExpectedDividend.Value;

                    var liabilites = transactionViewModel.Liabilities.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
                    stockViewModel.LiabilityValue = liabilites.Sum(x => x.Value.Value);
                    stockViewModel.MonthlyInterestPayment = liabilites.Sum(x => x.MonthlyInterestPayment);
                    stockViewModel.OriginalInterestPayment = liabilites.Sum(x => x.OriginalInterestPayment);
                    stockViewModel.MonthlyPayment = liabilites.Sum(x => x.TotalMonthlyPayment);
                    stockViewModel.AnnualPayment = stockViewModel.MonthlyPayment * 12;
                    stockViewModel.RemainedValue = liabilites.Sum(x => x.RemainedValue);
                }
                stockViewModel.InterestRate = stockViewModel.OriginalInterestPayment / stockViewModel.LiabilityValue * 12;

                result.StockSummaries.Add(stockViewModel);
            }

            result.TotalLiabilityValue = result.StockSummaries.Sum(x => x.LiabilityValue);
            result.TotalNumberOfStock = result.StockSummaries.Sum(x => x.NumberOfStock);
            result.TotalStockValue = result.StockSummaries.Sum(x => x.StockValue);
            result.TotalMonthlyInterestPayment = result.StockSummaries.Sum(x => x.MonthlyInterestPayment);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.StockSummaries.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            result.TotalMonthlyPayment = result.StockSummaries.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.StockSummaries.Sum(x => x.AnnualPayment);
            result.TotalRemainedValue = result.StockSummaries.Sum(x => x.RemainedValue);

            return result;
        }

        public static StockUpdateViewModel GetStockById(int id)
        {
            StockUpdateViewModel viewmodel = new StockUpdateViewModel();
            Entities entities = new Entities();
            var stock = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            viewmodel.Id = stock.Id;
            viewmodel.Name = stock.AssetName;
            viewmodel.Note = stock.Note;

            var transaction = entities.StockTransactions.Where(x => x.AssetId == id && !x.DisabledDate.HasValue).FirstOrDefault();
            viewmodel.NumberOfStock = transaction.NumberOfShares;
            viewmodel.SpotRice = transaction.SpotPrice;
            viewmodel.StockValue = transaction.Value;
            viewmodel.ExpectedDividend = transaction.ExpectedDividend;

            //var liability = entities.Liabilities.Where(x => x.TransactionId == transaction.Id && !x.DisabledDate.HasValue).FirstOrDefault();
            //viewmodel.Liabilities.Liabilities.Add(new StockLiabilityCreateViewModel
            //{
            //    Id = liability.Id,
            //    Value = liability.Value,
            //    Source = liability.Name,
            //    InterestRate = liability.InterestRate / 100,
            //    InterestType = liability.InterestType.Value,
            //    InterestRatePerX = liability.InterestRatePerX,
            //    StartDate = liability.StartDate,
            //    EndDate = liability.EndDate
            //});
            return viewmodel;
        }

        public static double GetStockValue(int id)
        {
            Entities entities = new Entities();
            var stock = entities.StockTransactions.Where(x => x.AssetId == id && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
            return stock;
        }

        public static int CreateStock(StockCreateViewModel model, string username)
        {
            int result = 0;
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            //Create stock
            Assets stock = new Assets();
            stock.AssetName = model.Name;
            stock.Note = model.Note;
            stock.CreatedDate = current;
            stock.CreatedBy = Constants.Constants.USER;
            stock.AssetType = (int)Constants.Constants.ASSET_TYPE.STOCK;
            stock.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.CREATE;
            stock.Username = username;

            StockTransactions transaction = new StockTransactions();
            transaction.Name = "Tạo cổ phiếu " + stock.AssetName;
            transaction.NumberOfShares = model.NumberOfStock.Value;
            transaction.SpotPrice = model.SpotRice.Value;
            transaction.Value = model.StockValue.Value;
            transaction.ExpectedDividend = model.ExpectedDividend.Value;
            transaction.Username = username;
            transaction.TransactionDate = current;
            transaction.TransactionType = (int)Constants.Constants.TRANSACTION_TYPE.CREATE;
            transaction.CreatedDate = current;
            transaction.CreatedBy = Constants.Constants.USER;

            if (model.IsInDebt)
            {
                if (model.Liabilities != null && model.Liabilities.Liabilities.Count > 0)
                {
                    foreach (var liabilityViewModel in model.Liabilities.Liabilities)
                    {
                        Liabilities liability = new Liabilities();
                        liability.Name = liabilityViewModel.Source;
                        liability.Value = liabilityViewModel.Value.Value;
                        liability.InterestType = liabilityViewModel.InterestType;
                        liability.InterestRate = liabilityViewModel.InterestRate.Value;
                        liability.InterestRatePerX = liabilityViewModel.InterestRatePerX;
                        liability.StartDate = liabilityViewModel.StartDate.Value;
                        liability.EndDate = liabilityViewModel.EndDate.Value;
                        liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.STOCK;
                        liability.CreatedDate = current;
                        liability.CreatedBy = Constants.Constants.USER;
                        liability.Username = username;
                        transaction.Liabilities.Add(liability);
                        stock.Liabilities.Add(liability);
                    }
                }
            }

            stock.StockTransactions.Add(transaction);

            entities.Assets.Add(stock);
            result = entities.SaveChanges();
            return result;
        }

        public static int UpdateStock(StockUpdateViewModel model)
        {
            Entities entities = new Entities();
            var stock = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();

            stock.AssetName = model.Name;
            stock.Note = model.Note;
            entities.Assets.Attach(stock);
            entities.Entry(stock).State = System.Data.Entity.EntityState.Modified;

            StockTransactions transaction = entities.StockTransactions.Where(x => x.AssetId == model.Id).FirstOrDefault();
            transaction.Name = "Tạo cổ phiếu " + stock.AssetName;
            transaction.NumberOfShares = model.NumberOfStock.Value;
            transaction.SpotPrice = model.SpotRice.Value;
            transaction.Value = model.StockValue.Value;
            transaction.ExpectedDividend = model.ExpectedDividend.Value;
            entities.StockTransactions.Attach(transaction);
            entities.Entry(transaction).State = System.Data.Entity.EntityState.Modified;

            return entities.SaveChanges();
        }

        public static int DeleteStock(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var stock = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            stock.DisabledDate = current;
            stock.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(stock);
            entities.Entry(stock).State = System.Data.Entity.EntityState.Modified;

            foreach (var transaction in entities.StockTransactions.Where(x => x.AssetId == id && !x.DisabledDate.HasValue))
            {
                transaction.DisabledDate = current;
                transaction.DisabledBy = Constants.Constants.USER;
                entities.StockTransactions.Attach(transaction);
                entities.Entry(transaction).State = System.Data.Entity.EntityState.Modified;

                foreach (var liability in entities.Liabilities.Where(x => x.TransactionId == transaction.Id && !x.DisabledDate.HasValue))
                {
                    liability.DisabledDate = current;
                    liability.DisabledBy = Constants.Constants.USER;
                    entities.Liabilities.Attach(liability);
                    entities.Entry(liability).State = System.Data.Entity.EntityState.Modified;
                }
            }

            return entities.SaveChanges();
        }
    }
}