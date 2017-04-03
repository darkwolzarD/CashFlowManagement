using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class DividendQueries
    {
        public static List<StockCodes> GetStockCodesByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<StockCodes> result = entities.StockCodes.Where(x => x.Username.Equals(username) && !x.EndDate.HasValue)
                                                                        .Select(x => new
                                                                        {
                                                                            x,
                                                                            StockTransactions = x.StockTransactions.Where(m => !m.EndDate.HasValue)
                                                                        }).AsEnumerable().Select(m => m.x).OrderByDescending(x => x.StockCode).ToList();
            return result;
        }

        public static int CreateStockCode(StockCodes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.StockCodes.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateStockCode(StockCodes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockCodes stockCode = entities.StockCodes.Where(x => x.Id == data.Id).Include(x => x.StockTransactions).FirstOrDefault();
            DateTime current = DateTime.Now;

            StockCodes updated_stockCode = new StockCodes();
            updated_stockCode.StockCode = data.StockCode;
            updated_stockCode.Username = stockCode.Username;

            foreach (var transaction in stockCode.StockTransactions)
            {
                updated_stockCode.StockTransactions.Add(new StockTransactions
                {
                    TransactionType = transaction.TransactionType,
                    TradeDay = transaction.TradeDay,
                    NumberOfShares = transaction.NumberOfShares,
                    SpotPrice = transaction.SpotPrice,
                    ExpectedDiviend = transaction.ExpectedDiviend,
                    MortgageValue = transaction.MortgageValue,
                    ExpenseInterest = transaction.ExpenseInterest,
                    StartDate = transaction.StartDate,
                    EndDate = transaction.EndDate,
                    Note = transaction.Note
                });
            }
            entities.StockCodes.Add(updated_stockCode);

            stockCode.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.StockCodes.Attach(stockCode);
            var entry = entities.Entry(stockCode);
            entry.Property(x => x.EndDate).IsModified = true;

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteStockCode(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockCodes stockCode = entities.StockCodes.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            stockCode.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.StockCodes.Attach(stockCode);
            var entry = entities.Entry(stockCode);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static StockCodes GetStockCodeById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockCodes stockCode = entities.StockCodes.Where(x => x.Id == id).FirstOrDefault();
            return stockCode;
        }

        public static int CreateTransaction(StockTransactions data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.StockTransactions.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteTransaction(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockTransactions transaction = entities.StockTransactions.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            transaction.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.StockTransactions.Attach(transaction);
            var entry = entities.Entry(transaction);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static StockTransactions GetTransactionById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockTransactions transaction = entities.StockTransactions.Where(x => x.Id == id).FirstOrDefault();
            return transaction;
        }

        public static int UpdateTransaction(StockTransactions data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            StockTransactions transaction = entities.StockTransactions.Where(x => x.Id == data.Id).FirstOrDefault();
            DateTime current = DateTime.Now;

            transaction.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.StockTransactions.Attach(transaction);
            var entry = entities.Entry(transaction);
            entry.Property(x => x.EndDate).IsModified = true;

            StockTransactions updated_transaction = new StockTransactions();
            updated_transaction.TransactionType = data.TransactionType;
            updated_transaction.TradeDay = data.TradeDay;
            updated_transaction.NumberOfShares = data.NumberOfShares;
            updated_transaction.SpotPrice = data.SpotPrice;
            updated_transaction.StartDate = new DateTime(current.Year, current.Month, 1);
            updated_transaction.EndDate = data.EndDate;
            updated_transaction.ExpectedDiviend = data.ExpectedDiviend;
            updated_transaction.MortgageValue = data.MortgageValue;
            updated_transaction.ExpectedDiviend = data.ExpenseInterest;
            updated_transaction.Note = data.Note;
            updated_transaction.StockId = data.StockId;
            entities.StockTransactions.Add(updated_transaction);

            int result = entities.SaveChanges();
            return result;
        }
    }
}