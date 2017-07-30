using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class FamilyExpenseQueries
    {
        public static FamilyExpenseListViewModel GetFamilyExpenseByUser(string username)
        {
            Entities entities = new Entities();
            var familyExpenses = entities.Expenses.Where(x => x.Username.Equals(username) 
                                                && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.FAMILY 
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            FamilyExpenseListViewModel result = new FamilyExpenseListViewModel();
            foreach (var familyExpense in familyExpenses)
            {
                FamilyExpenseViewModel viewModel = new FamilyExpenseViewModel
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
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static FamilyExpenseSummaryListViewModel GetFamilyExpenseSummaryByUser(string username)
        {
            Entities entities = new Entities();
            var familyExpenses = entities.Expenses.Where(x => x.Username.Equals(username)
                                                && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.FAMILY
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name);
            FamilyExpenseSummaryListViewModel result = new FamilyExpenseSummaryListViewModel();
            foreach (var familyExpense in familyExpenses)
            {
                FamilyExpenseSummaryViewModel viewModel = new FamilyExpenseSummaryViewModel
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

        public static FamilyExpenseUpdateViewModel GetFamilyExpenseById(int id)
        {
            Entities entities = new Entities();
            Expenses familyExpense = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            FamilyExpenseUpdateViewModel model = new FamilyExpenseUpdateViewModel
            {
                Id = familyExpense.Id,
                Expense = familyExpense.Value,
                ExpenseDay = familyExpense.ExpenseDay,
                Note = familyExpense.Note,
                Source = familyExpense.Name
            };
            return model;
        }

        public static int CreateFamilyExpense(FamilyExpenseCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses familyExpense = new Expenses();
            familyExpense.Name = model.Source;
            familyExpense.ExpenseDay = model.ExpenseDay.Value;
            familyExpense.Value = model.Expense.Value;
            familyExpense.Note = model.Note;
            familyExpense.ExpenseType = (int)Constants.Constants.EXPENSE_TYPE.FAMILY;
            familyExpense.StartDate = current;
            familyExpense.CreatedDate = current;
            familyExpense.CreatedBy = Constants.Constants.USER;
            familyExpense.Username = username;

            entities.Expenses.Add(familyExpense);
            return entities.SaveChanges();
        }

        public static int UpdateFamilyExpense(FamilyExpenseUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses familyExpense = entities.Expenses.Where(x => x.Id == model.Id).FirstOrDefault();
            familyExpense.Name = model.Source;
            familyExpense.ExpenseDay = model.ExpenseDay.Value;
            familyExpense.Value = model.Expense.Value;
            familyExpense.Note = model.Note;

            entities.Expenses.Attach(familyExpense);
            entities.Entry(familyExpense).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteFamilyExpense(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Expenses familyExpense = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            familyExpense.DisabledDate = current;
            familyExpense.DisabledBy = Constants.Constants.USER;
            entities.Expenses.Attach(familyExpense);
            entities.Entry(familyExpense).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }
    }
}