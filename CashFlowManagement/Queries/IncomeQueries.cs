using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class IncomeQueries
    {
        public static IncomeListViewModel GetIncomeByUser(string username, int type)
        {
            Entities entities = new Entities();
            List<Incomes> queryResult = entities.Incomes.Where(x => x.Username.Equals(username) && x.IncomeType == type && !x.DisabledDate.HasValue).ToList();
            IncomeListViewModel result = new IncomeListViewModel
            {
                List = queryResult,
                Type = type,
                TotalMonthlyIncome = queryResult.Sum(x => x.Value)
            };
            return result;
        }

        public static Incomes GetIncomeById(int id)
        {
            Entities entities = new Entities();
            Incomes result = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        public static int CreateIncome(Incomes income, int type, string username)
        {
            Entities entities = new Entities();
            income.CreatedDate = DateTime.Now;
            income.IncomeType = type;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;

            entities.Incomes.Add(income);

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.ADD, "thu nhập \"" + income.Name + "\"", username, income.Value, DateTime.Now);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateIncome(Incomes model, string username)
        {
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == model.Id).FirstOrDefault();
            income.DisabledDate = DateTime.Now;
            income.DisabledBy = Constants.Constants.USER;
            entities.Incomes.Attach(income);
            var entry = entities.Entry(income);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.DisabledBy).IsModified = true;

            Incomes updated_income = new Incomes();
            updated_income.Value = model.Value;
            updated_income.Name = model.Name;
            updated_income.IncomeDay = model.IncomeDay;
            updated_income.StartDate = model.StartDate;
            updated_income.EndDate = model.EndDate;
            updated_income.IncomeType = model.IncomeType;
            updated_income.Note = model.Note;
            updated_income.CreatedDate = DateTime.Now;
            updated_income.CreatedBy = Constants.Constants.USER;
            updated_income.Username = username;

            entities.Incomes.Add(updated_income);

            var after_cashflows = entities.Assets.Where(x => x.Username.Equals(username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                && x.AssetName.StartsWith("CashFlow") && !x.DisabledDate.HasValue && x.StartDate.Value.Month >= model.StartDate.Month
                                                && x.StartDate.Value.Year == model.StartDate.Year && (model.EndDate.HasValue ? x.StartDate.Value.Month <= model.EndDate.Value.Month
                                                && x.StartDate.Value.Year == model.EndDate.Value.Year : true));

            foreach (var cashflow in after_cashflows)
            {
                cashflow.DisabledDate = DateTime.Now;
                cashflow.DisabledBy = Constants.Constants.SYSTEM;
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;

                Assets updated_cashflow = new Assets();
                updated_cashflow.AssetName = cashflow.AssetName;
                updated_cashflow.StartDate = cashflow.StartDate;
                updated_cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                updated_cashflow.CreatedBy = Constants.Constants.SYSTEM;
                updated_cashflow.CreatedDate = DateTime.Now;
                updated_cashflow.Value = cashflow.Value + updated_income.Value - income.Value;
                updated_cashflow.Username = income.Username;

                entities.Assets.Add(updated_cashflow);
            }

            var previous_cashflows = entities.Assets.Where(x => x.Username.Equals(username) && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                && x.AssetName.StartsWith("CashFlow") && !x.DisabledDate.HasValue && x.StartDate.Value.Month <= model.StartDate.Month
                                                && x.StartDate.Value.Year == model.StartDate.Year);

            foreach (var cashflow in previous_cashflows)
            {
                cashflow.DisabledDate = DateTime.Now;
                cashflow.DisabledBy = Constants.Constants.SYSTEM;
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;

                Assets updated_cashflow = new Assets();
                updated_cashflow.AssetName = cashflow.AssetName;
                updated_cashflow.StartDate = cashflow.StartDate;
                updated_cashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                updated_cashflow.CreatedBy = Constants.Constants.SYSTEM;
                updated_cashflow.CreatedDate = DateTime.Now;
                updated_cashflow.Value = cashflow.Value - income.Value;
                updated_cashflow.Username = income.Username;

                entities.Assets.Add(updated_cashflow);
            }

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.UPDATE, "thu nhập \"" + income.Name + "\"", username, income.Value, DateTime.Now);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteIncome(int id)
        {
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == id && !x.DisabledDate.HasValue).FirstOrDefault();
            income.DisabledDate = DateTime.Now;
            income.DisabledBy = Constants.Constants.USER;

            entities.Incomes.Attach(income);
            var entry = entities.Entry(income);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.DisabledBy).IsModified = true;

            Log log = LogQueries.CreateLog((int)Constants.Constants.LOG_TYPE.DELETE, "thu nhập \"" + income.Name + "\"", income.Username, income.Value, DateTime.Now);
            entities.Log.Add(log);

            int result = entities.SaveChanges();
            return result;
        }
    }
}