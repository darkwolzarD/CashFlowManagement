﻿using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class ExpenseQueries
    {
        public static ExpenseListViewModel GetExpenseByUser(string username, int type)
        {
            Entities entities = new Entities();
            List<Expenses> queryResult = entities.Expenses.Where(x => x.Username.Equals(username) && x.ExpenseType == type && !x.DisabledDate.HasValue).ToList();
            ExpenseListViewModel result = new ExpenseListViewModel
            {
                List = queryResult,
                Type = type
            };
            return result;
        }

        public static Expenses GetExpenseById(int id)
        {
            Entities entities = new Entities();
            Expenses result = entities.Expenses.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        public static int CreateExpense(Expenses expense, int type, string username)
        {
            Entities entities = new Entities();
            expense.CreatedDate = DateTime.Now;
            expense.ExpenseType = type;
            expense.Username = username;
            expense.CreatedBy = Constants.Constants.USER;

            entities.Expenses.Add(expense);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateExpense(Expenses model, string username)
        {
            Entities entities = new Entities();

            Expenses expense = entities.Expenses.Where(x => x.Id == model.Id).FirstOrDefault();
            expense.DisabledDate = DateTime.Now;
            expense.DisabledBy = Constants.Constants.USER;
            entities.Expenses.Attach(expense);
            var entry = entities.Entry(expense);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.DisabledBy).IsModified = true;

            Expenses updated_expense = new Expenses();
            updated_expense.Value = model.Value;
            updated_expense.Name = model.Name;
            updated_expense.ExpenseType = model.ExpenseType;
            updated_expense.CreatedDate = DateTime.Now;
            updated_expense.CreatedBy = Constants.Constants.USER;
            updated_expense.Username = username;

            entities.Expenses.Add(updated_expense);
            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteExpense(int id)
        {
            Entities entities = new Entities();

            Expenses expense = entities.Expenses.Where(x => x.Id == id && !x.DisabledDate.HasValue).FirstOrDefault();
            expense.DisabledDate = DateTime.Now;
            expense.DisabledBy = Constants.Constants.USER;

            entities.Expenses.Attach(expense);
            var entry = entities.Entry(expense);
            entry.Property(x => x.DisabledDate).IsModified = true;
            entry.Property(x => x.DisabledBy).IsModified = true;

            int result = entities.SaveChanges();
            return result;
        }
    }
}