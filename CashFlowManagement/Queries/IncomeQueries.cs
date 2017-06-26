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
        /// <summary>
        /// Get all active incomes of an account by income type
        /// </summary>
        /// <param name="username">Username of account</param>
        /// <param name="type">Income type to filter</param>
        /// <returns>List of active incomes</returns>
        public static IncomeListViewModel GetIncomeByUser(string username, int type)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;
            List<Incomes> queryResult = entities.Incomes.Where(x => x.Username.Equals(username) && x.IncomeType == type && !x.DisabledDate.HasValue).OrderBy(x => x.Name).ToList();
            IncomeListViewModel result = new IncomeListViewModel
            {
                List = queryResult,
                Type = type,
                TotalMonthlyIncome = queryResult.Where(x => x.StartDate <= current && (x.EndDate.HasValue ? x.EndDate.Value >= current : true)).Sum(x => x.Value)
            };
            return result;
        }

        /// <summary>
        /// Get an income by unique identifier
        /// </summary>
        /// <param name="id">Unique identifier</param>
        /// <returns>Income</returns>
        public static Incomes GetIncomeById(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;
            Incomes result = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Create new income for an account
        /// </summary>
        /// <param name="income">Income information</param>
        /// <param name="type">Income type</param>
        /// <param name="username">Username of account</param>
        /// <returns>Create result</returns>
        public static int CreateIncome(Incomes income, int type, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;

            Entities entities = new Entities();

            //Create income
            income.Id = -1;
            income.StartDate = new DateTime(income.StartDate.Year, income.StartDate.Month, income.IncomeDay.Value);
            if (income.EndDate.HasValue)
            {
                income.EndDate = new DateTime(income.EndDate.Value.Year, income.EndDate.Value.Month, income.IncomeDay.Value);
            }
            income.CreatedDate = current;
            income.IncomeType = type;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;
            entities.Incomes.Add(income);

            //Create income creation log
            History incomeHistory = new History();
            incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
            incomeHistory.Content = "Tạo mới " + income.Name;
            incomeHistory.CreatedDate = current;
            incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.CREATE;
            incomeHistory.CreatedBy = Constants.Constants.USER;
            incomeHistory.Username = username;
            incomeHistory.Incomes = income;
            entities.History.Add(incomeHistory);

            //Create income cashflow
            DateTime cashflowStartDate = new DateTime(income.StartDate.Year, income.StartDate.Month, 1);
            cashflowStartDate = cashflowStartDate < userCreateDate ? userCreateDate : cashflowStartDate;
            DateTime cashflowEndDate = current;
            if (income.EndDate.HasValue)
            {
                if (income.EndDate < current)
                {
                    cashflowEndDate = income.EndDate.Value;
                }
            }

            while (cashflowStartDate <= cashflowEndDate)
            {
                Assets monthlyCashflow = new Assets();
                monthlyCashflow.AssetName = "Thu nhập từ " + income.Name;
                monthlyCashflow.Value = income.Value;
                monthlyCashflow.StartDate = cashflowStartDate;
                monthlyCashflow.CreatedDate = current;
                monthlyCashflow.CreatedBy = Constants.Constants.SYSTEM;
                monthlyCashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                monthlyCashflow.Username = username;
                monthlyCashflow.Incomes = income;
                entities.Assets.Add(monthlyCashflow);

                History cashflowHistory = new History();
                cashflowHistory.Type = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                cashflowHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.CREATE;
                cashflowHistory.Content = "Tạo mới thu nhập từ " + income.Name + " tháng " + cashflowStartDate.ToString("MM/yyyy");
                cashflowHistory.CreatedDate = current;
                cashflowHistory.CreatedBy = Constants.Constants.SYSTEM;
                cashflowHistory.Username = username;
                cashflowHistory.Assets = monthlyCashflow;
                entities.History.Add(cashflowHistory);

                cashflowStartDate = cashflowStartDate.AddMonths(1);
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static int InitializeIncome(Incomes income, int type, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;

            Entities entities = new Entities();

            //Create income
            income.Id = -1;
            income.StartDate = new DateTime(income.StartDate.Year, income.StartDate.Month, income.IncomeDay.Value);
            if (income.EndDate.HasValue)
            {
                income.EndDate = new DateTime(income.EndDate.Value.Year, income.EndDate.Value.Month, income.IncomeDay.Value);
            }
            income.CreatedDate = current;
            income.IncomeType = type;
            income.Username = username;
            income.CreatedBy = Constants.Constants.USER;
            entities.Incomes.Add(income);

            int result = entities.SaveChanges();
            return result;
        }

        /// <summary>
        /// Update income
        /// </summary>
        /// <param name="model">Income updated information</param>
        /// <param name="username">Username of account</param>
        /// <returns>Update status</returns>
        public static int UpdateIncome(Incomes model, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == model.Id).FirstOrDefault();

            if (!income.Name.Equals(model.Name))
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Name";
                incomeHistory.OldValue = income.Name;
                incomeHistory.NewValue = model.Name;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.Name = model.Name;
            }

            if (income.Value != model.Value)
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Value";
                incomeHistory.OldValue = income.Value.ToString();
                incomeHistory.NewValue = model.Value.ToString();
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.Value = model.Value;
            }

            if (income.Note != model.Note)
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Note";
                incomeHistory.OldValue = income.Note;
                incomeHistory.NewValue = model.Note;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.Note = model.Note;
            }

            if (!income.StartDate.Equals(model.StartDate))
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "StartDate";
                incomeHistory.OldValue = income.StartDate.ToString("MM/yyyy");
                incomeHistory.NewValue = model.StartDate.ToString("MM/yyyy");
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.StartDate = model.StartDate;
            }

            if (income.IncomeDay != model.IncomeDay)
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "IncomeDay";
                incomeHistory.OldValue = income.IncomeDay.ToString();
                incomeHistory.NewValue = model.IncomeDay.ToString();
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.IncomeDay = model.IncomeDay;
                income.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.IncomeDay.Value);
            }

            if (!income.EndDate.Equals(model.EndDate))
            {
                History incomeHistory = new History();
                incomeHistory.Type = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
                incomeHistory.Content = "Cập nhật " + income.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "EndDate";
                incomeHistory.OldValue = income.EndDate.HasValue ? income.EndDate.Value.ToString("MM/yyyy") : string.Empty;
                incomeHistory.NewValue = model.EndDate.HasValue ? model.EndDate.Value.ToString("MM/yyyy") : string.Empty;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Incomes = income;
                entities.History.Add(incomeHistory);

                income.EndDate = model.EndDate;
            }

            foreach (var cashflow in income.Assets.Where(x => !x.DisabledDate.HasValue))
            {
                if (cashflow.StartDate <= (income.EndDate.HasValue ? income.EndDate.Value : current))
                {
                    cashflow.Value = income.Value;
                    cashflow.StartDate = income.StartDate;
                }
                else
                {
                    cashflow.DisabledDate = current;
                    cashflow.DisabledBy = Constants.Constants.SYSTEM;
                }
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;
            }

            entities.Incomes.Attach(income);
            entities.Entry(income).State = EntityState.Modified;
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateInitializeIncome(Incomes model, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == model.Id).FirstOrDefault();

            if (!income.Name.Equals(model.Name))
            {
                income.Name = model.Name;
            }

            if (income.Value != model.Value)
            {
                income.Value = model.Value;
            }

            if (income.Note != model.Note)
            {
                income.Note = model.Note;
            }

            if (!income.StartDate.Equals(model.StartDate))
            {
                income.StartDate = model.StartDate;
            }

            if (income.IncomeDay != model.IncomeDay)
            {
                income.IncomeDay = model.IncomeDay;
                income.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.IncomeDay.Value);
            }

            if (!income.EndDate.Equals(model.EndDate))
            {
                income.EndDate = model.EndDate;
            }

            entities.Incomes.Attach(income);
            entities.Entry(income).State = EntityState.Modified;
            int result = entities.SaveChanges();
            return result;
        }

        /// <summary>
        /// Delete income
        /// </summary>
        /// <param name="id">Unique identifier of income</param>
        /// <returns>Delete status</returns>
        public static int DeleteIncome(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            income.DisabledDate = current;
            income.DisabledBy = Constants.Constants.USER;
            entities.Incomes.Attach(income);
            entities.Entry(income).State = EntityState.Modified;

            foreach (var cashflow in income.Assets.Where(x => !x.DisabledDate.HasValue))
            {
                cashflow.DisabledDate = current;
                cashflow.DisabledBy = Constants.Constants.SYSTEM;
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteInitializeIncome(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Incomes income = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            income.DisabledDate = current;
            income.DisabledBy = Constants.Constants.USER;
            entities.Incomes.Attach(income);
            entities.Entry(income).State = EntityState.Modified;

            int result = entities.SaveChanges();
            return result;
        }

        public static CashFlowDetailListViewModel GetCashFlowDetail(Incomes income, int? id, string username)
        {
            CashFlowDetailListViewModel result = new CashFlowDetailListViewModel();
            DateTime current = DateTime.Now;
            DateTime userCreatedDate = UserQueries.GetUserByUsername(username).CreatedDate;
            Incomes dbIncome = null;
            result.BeforeAvailableMoney = AssetQueries.CheckAvailableMoney(username, current);
            result.AfterAvailableMoney = result.BeforeAvailableMoney;
            DateTime startDate = income.StartDate < userCreatedDate ? userCreatedDate : income.StartDate;
            DateTime endDate = current;

            if (id > 0)
            {
                income = IncomeQueries.GetIncomeById(id.Value);

                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel
                    {
                        Month = startDate.ToString("MM/yyyy"),
                        IncomeBefore = income.Value,
                        IncomeAfter = 0
                    };
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney -= income.Value;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Delete";
            }
            else if (income.Id == 0)
            {
                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel
                    {
                        Month = startDate.ToString("MM/yyyy"),
                        IncomeBefore = 0,
                        IncomeAfter = income.Value
                    };
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney += income.Value;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Create";
            }
            else
            {
                dbIncome = IncomeQueries.GetIncomeById(income.Id);

                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel();
                    model.Month = startDate.ToString("MM/yyyy");
                    if (startDate >= dbIncome.StartDate && startDate <= (dbIncome.EndDate.HasValue ? dbIncome.EndDate : current))
                    {
                        model.IncomeBefore = dbIncome.Value;
                    }
                    else
                    {
                        model.IncomeBefore = 0;
                    }
                    if (startDate >= income.StartDate && startDate <= (income.EndDate.HasValue ? income.EndDate : current))
                    {
                        model.IncomeAfter = income.Value;
                    }
                    else
                    {
                        model.IncomeAfter = 0;
                    }
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney += model.IncomeAfter - model.IncomeBefore;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Update";
            }
            return result;
        }

        public static string GetStartDate(string name, string username)
        {
            Entities entities = new Entities();
            var income = entities.Incomes.Where(x => x.Name.Equals(name) && x.Username.Equals(username) && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate);
            if (income.Any())
            {
                if (income.FirstOrDefault().EndDate.HasValue)
                {
                    return income.FirstOrDefault().EndDate.Value.AddMonths(1).ToString("MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}