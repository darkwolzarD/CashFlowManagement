using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class ExpenseQueries
    {
        /// <summary>
        /// Get all active expenses of an account by expense type
        /// </summary>
        /// <param name="username">Username of account</param>
        /// <param name="type">Income type to filter</param>
        /// <returns>List of active expenses</returns>
        public static ExpenseListViewModel GetExpenseByUser(string username, int type)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;
            List<Expenses> queryResult = entities.Expenses.Where(x => x.Username.Equals(username)
                                        && x.ExpenseType == type
                                        && !x.DisabledDate.HasValue).ToList();
            ExpenseListViewModel result = new ExpenseListViewModel
            {
                List = queryResult,
                Type = type,
                TotalMonthlyExpense = queryResult.Where(x => x.StartDate <= current && (x.EndDate.HasValue ? x.EndDate.Value >= current : true)).Select(x => x.Value).DefaultIfEmpty(0).Sum()
            };
            return result;
        }

        /// <summary>
        /// Get an expense by unique identifier
        /// </summary>
        /// <param name="id">Unique identifier</param>
        /// <returns>Expense</returns>
        public static Expenses GetExpenseById(int id)
        {
            Entities entities = new Entities();
            Expenses result = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        public static int CreateExpense(Expenses expense, int type, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;

            Entities entities = new Entities();

            //Create income
            expense.Id = -1;
            expense.StartDate = new DateTime(expense.StartDate.Year, expense.StartDate.Month, expense.ExpenseDay);
            if (expense.EndDate.HasValue)
            {
                expense.EndDate = new DateTime(expense.EndDate.Value.Year, expense.EndDate.Value.Month, expense.ExpenseDay);
            }
            expense.CreatedDate = current;
            expense.Username = username;
            expense.CreatedBy = Constants.Constants.USER;
            entities.Expenses.Add(expense);

            //Create income creation log
            History incomeHistory = new History();
            incomeHistory.Type = type;
            incomeHistory.Content = "Tạo mới " + expense.Name;
            incomeHistory.CreatedDate = current;
            incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.CREATE;
            incomeHistory.CreatedBy = Constants.Constants.USER;
            incomeHistory.Username = username;
            incomeHistory.Expenses = expense;
            entities.History.Add(incomeHistory);

            //Create income cashflow
            DateTime cashflowStartDate = new DateTime(expense.StartDate.Year, expense.StartDate.Month, 1);
            cashflowStartDate = cashflowStartDate < userCreateDate ? userCreateDate : cashflowStartDate;
            DateTime cashflowEndDate = current;
            if (expense.EndDate.HasValue)
            {
                if (expense.EndDate < current)
                {
                    cashflowEndDate = expense.EndDate.Value;
                }
            }

            while (cashflowStartDate <= cashflowEndDate)
            {
                Assets monthlyCashflow = new Assets();
                monthlyCashflow.AssetName = "Chi tiêu từ " + expense.Name;
                monthlyCashflow.Value = -expense.Value;
                monthlyCashflow.StartDate = cashflowStartDate;
                monthlyCashflow.CreatedDate = current;
                monthlyCashflow.CreatedBy = Constants.Constants.SYSTEM;
                monthlyCashflow.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                monthlyCashflow.Username = username;
                monthlyCashflow.Expenses = expense;
                entities.Assets.Add(monthlyCashflow);

                History cashflowHistory = new History();
                cashflowHistory.Type = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                cashflowHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.CREATE;
                cashflowHistory.Content = "Tạo mới chi tiêu từ " + expense.Name + " tháng " + cashflowStartDate.ToString("MM/yyyy");
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

        /// <summary>
        /// Update expense
        /// </summary>
        /// <param name="model">Expense updated information</param>
        /// <param name="username">Username of account</param>
        /// <returns>Update status</returns>
        public static int UpdateExpense(Expenses model, string username)
        {
            DateTime current = DateTime.Now;
            DateTime userCreateDate = UserQueries.GetUserByUsername(username).CreatedDate;
            Entities entities = new Entities();

            Expenses expense = entities.Expenses.Where(x => x.Id == model.Id).FirstOrDefault();

            if (!expense.Name.Equals(model.Name))
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Name";
                incomeHistory.OldValue = expense.Name;
                incomeHistory.NewValue = model.Name;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.Name = model.Name;
            }

            if (expense.Value != model.Value)
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Value";
                incomeHistory.OldValue = expense.Value.ToString();
                incomeHistory.NewValue = model.Value.ToString();
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.Value = model.Value;
            }

            if (expense.Note != model.Note)
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "Note";
                incomeHistory.OldValue = expense.Note;
                incomeHistory.NewValue = model.Note;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.Note = model.Note;
            }

            if (!expense.StartDate.Equals(model.StartDate))
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "StartDate";
                incomeHistory.OldValue = expense.StartDate.ToString("MM/yyyy");
                incomeHistory.NewValue = model.StartDate.ToString("MM/yyyy");
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.StartDate = model.StartDate;
            }

            if (expense.ExpenseDay != model.ExpenseDay)
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "IncomeDay";
                incomeHistory.OldValue = expense.ExpenseDay.ToString();
                incomeHistory.NewValue = model.ExpenseDay.ToString();
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.ExpenseDay = model.ExpenseDay;
                expense.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.ExpenseDay);
            }

            if (!expense.EndDate.Equals(model.EndDate))
            {
                History incomeHistory = new History();
                incomeHistory.Type = expense.ExpenseType;
                incomeHistory.Content = "Cập nhật " + expense.Name;
                incomeHistory.CreatedDate = current;
                incomeHistory.ActionType = (int)Constants.Constants.HISTORY_TYPE.UPDATE;
                incomeHistory.Field = "EndDate";
                incomeHistory.OldValue = expense.EndDate.HasValue ? expense.EndDate.Value.ToString("MM/yyyy") : string.Empty;
                incomeHistory.NewValue = model.EndDate.HasValue ? model.EndDate.Value.ToString("MM/yyyy") : string.Empty;
                incomeHistory.CreatedBy = Constants.Constants.USER;
                incomeHistory.Username = username;
                incomeHistory.Expenses = expense;
                entities.History.Add(incomeHistory);

                expense.EndDate = model.EndDate;
            }

            foreach (var cashflow in expense.Assets.Where(x => !x.DisabledDate.HasValue))
            {
                if (cashflow.StartDate <= (expense.EndDate.HasValue ? expense.EndDate.Value : current))
                {
                    cashflow.Value = -expense.Value;
                    cashflow.StartDate = expense.StartDate;
                }
                else
                {
                    cashflow.DisabledDate = current;
                    cashflow.DisabledBy = Constants.Constants.SYSTEM;
                }
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;
            }

            entities.Expenses.Attach(expense);
            entities.Entry(expense).State = EntityState.Modified;
            int result = entities.SaveChanges();
            return result;
        }

        /// <summary>
        /// Delete expense
        /// </summary>
        /// <param name="id">Unique identifier of expense</param>
        /// <returns>Delete status</returns>
        public static int DeleteExpense(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            Expenses expense = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            expense.DisabledDate = current;
            expense.DisabledBy = Constants.Constants.USER;
            entities.Expenses.Attach(expense);
            entities.Entry(expense).State = EntityState.Modified;

            foreach (var cashflow in expense.Assets.Where(x => !x.DisabledDate.HasValue))
            {
                cashflow.DisabledDate = current;
                cashflow.DisabledBy = Constants.Constants.SYSTEM;
                entities.Assets.Attach(cashflow);
                entities.Entry(cashflow).State = EntityState.Modified;
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static CashFlowDetailListViewModel GetCashFlowDetail(Expenses expense, int? id, string username)
        {
            CashFlowDetailListViewModel result = new CashFlowDetailListViewModel();
            DateTime current = DateTime.Now;
            DateTime userCreatedDate = UserQueries.GetUserByUsername(username).CreatedDate;
            Expenses dbExpense = null;
            result.BeforeAvailableMoney = AssetQueries.CheckAvailableMoney(username, current);
            result.AfterAvailableMoney = result.BeforeAvailableMoney;
            DateTime startDate = expense.StartDate < userCreatedDate ? userCreatedDate : expense.StartDate;
            DateTime endDate = current;

            if (id > 0)
            {
                expense = ExpenseQueries.GetExpenseById(id.Value);

                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel
                    {
                        Month = startDate.ToString("MM/yyyy"),
                        IncomeBefore = 0 - expense.Value,
                        IncomeAfter = 0
                    };
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney += expense.Value;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Delete";
            }
            else if (expense.Id == 0)
            {
                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel
                    {
                        Month = startDate.ToString("MM/yyyy"),
                        IncomeBefore = 0,
                        IncomeAfter = 0 - expense.Value
                    };
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney -= expense.Value;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Create";
            }
            else
            {
                dbExpense = ExpenseQueries.GetExpenseById(expense.Id);

                while (startDate <= endDate)
                {
                    CashFlowDetailViewModel model = new CashFlowDetailViewModel();
                    model.Month = startDate.ToString("MM/yyyy");
                    if (startDate >= dbExpense.StartDate && startDate <= (dbExpense.EndDate.HasValue ? dbExpense.EndDate : current))
                    {
                        model.IncomeBefore = 0 - dbExpense.Value;
                    }
                    else
                    {
                        model.IncomeBefore = 0;
                    }
                    if (startDate >= expense.StartDate && startDate <= (expense.EndDate.HasValue ? expense.EndDate : current))
                    {
                        model.IncomeAfter = 0 - expense.Value;
                    }
                    else
                    {
                        model.IncomeAfter = 0;
                    }
                    result.CashflowDetails.Add(model);
                    result.AfterAvailableMoney -= model.IncomeAfter - model.IncomeBefore;
                    startDate = startDate.AddMonths(1);
                }
                result.Action = "Update";
            }
            return result;
        }

        public static string GetStartDate(string name, string username, int type)
        {
            Entities entities = new Entities();
            var income = entities.Expenses.Where(x => x.Name.Equals(name) && x.Username.Equals(username) && x.ExpenseType == type && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate);
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