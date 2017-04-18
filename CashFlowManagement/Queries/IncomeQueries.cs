using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
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
                Type = type
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
            updated_income.IncomeType = model.IncomeType;
            updated_income.Note = model.Note;
            updated_income.CreatedDate = DateTime.Now;
            updated_income.CreatedBy = Constants.Constants.USER;
            updated_income.Username = username;

            entities.Incomes.Add(updated_income);
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

            int result = entities.SaveChanges();
            return result;
        }
    }
}