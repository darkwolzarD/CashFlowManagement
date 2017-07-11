using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class OtherExpenseQueries
    {
        public static OtherExpenseListViewModel GetOtherExpenseByUser(string username)
        {
            Entities entities = new Entities();
            var otherExpenses = entities.Expenses.Where(x => x.Username.Equals(username) 
                                                && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.OTHERS 
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name).ToList();
            OtherExpenseListViewModel result = new OtherExpenseListViewModel();
            foreach (var otherExpense in otherExpenses)
            {
                OtherExpenseViewModel viewModel = new OtherExpenseViewModel
                {
                    Id = otherExpense.Id,
                    Source = otherExpense.Name,
                    ExpenseDay = otherExpense.ExpenseDay,
                    Expense = otherExpense.Value,
                    AnnualExpense = otherExpense.Value * 12,
                    Note = otherExpense.Note
                };

                result.Expenses.Add(viewModel);
            }

            result.TotalExpense = result.Expenses.Sum(x => x.Expense.Value);
            result.TotalAnnualExpense = result.TotalExpense * 12;
            return result;
        }

        public static OtherExpenseSummaryListViewModel GetOtherExpenseSummaryByUser(string username)
        {
            Entities entities = new Entities();
            var familyExpenses = entities.Expenses.Where(x => x.Username.Equals(username)
                                                && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.OTHERS
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            OtherExpenseSummaryListViewModel result = new OtherExpenseSummaryListViewModel();
            foreach (var familyExpense in familyExpenses)
            {
                OtherExpenseSummaryViewModel viewModel = new OtherExpenseSummaryViewModel
                {
                    Id = familyExpense.Id,
                    Source = familyExpense.Name,
                    ExpenseDay = familyExpense.ExpenseDay,
                    Expense = familyExpense.Value,
                    AnnualExpense = familyExpense.Value * 12,
                    Note = familyExpense.Note
                };

                result.Expenses.Add(viewModel);
            }

            result.TotalExpense = result.Expenses.Sum(x => x.Expense.Value);
            result.TotalAnnualExpense = result.TotalExpense * 12;
            return result;
        }

        public static OtherExpenseUpdateViewModel GetOtherExpenseById(int id)
        {
            Entities entities = new Entities();
            Expenses otherExpense = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            OtherExpenseUpdateViewModel model = new OtherExpenseUpdateViewModel
            {
                Id = otherExpense.Id,
                Expense = otherExpense.Value,
                ExpenseDay = otherExpense.ExpenseDay,
                Note = otherExpense.Note,
                Source = otherExpense.Name
            };
            return model;
        }

        public static int CreateOtherExpense(OtherExpenseCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses otherExpense = new Expenses();
            otherExpense.Name = model.Source;
            otherExpense.ExpenseDay = model.ExpenseDay.Value;
            otherExpense.Value = model.Expense.Value;
            otherExpense.Note = model.Note;
            otherExpense.ExpenseType = (int)Constants.Constants.EXPENSE_TYPE.OTHERS;
            otherExpense.StartDate = current;
            otherExpense.CreatedDate = current;
            otherExpense.CreatedBy = Constants.Constants.USER;
            otherExpense.Username = username;

            entities.Expenses.Add(otherExpense);
            return entities.SaveChanges();
        }

        public static int UpdateOtherExpense(OtherExpenseUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses otherExpense = entities.Expenses.Where(x => x.Id == model.Id).FirstOrDefault();
            otherExpense.Name = model.Source;
            otherExpense.ExpenseDay = model.ExpenseDay.Value;
            otherExpense.Value = model.Expense.Value;
            otherExpense.Note = model.Note;

            entities.Expenses.Attach(otherExpense);
            entities.Entry(otherExpense).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteOtherExpense(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses otherExpense = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            otherExpense.DisabledDate = current;
            otherExpense.DisabledBy = Constants.Constants.USER;
            entities.Expenses.Attach(otherExpense);
            entities.Entry(otherExpense).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }
    }
}