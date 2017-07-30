using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class SalaryQueries
    {
        public static SalaryListViewModel GetSalaryByUser(string username)
        {
            Entities entities = new Entities();
            var salaries = entities.Incomes.Where(x => x.Username.Equals(username) 
                                                && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME 
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name).ToList();
            SalaryListViewModel result = new SalaryListViewModel();
            foreach (var salary in salaries)
            {
                SalaryViewModel viewModel = new SalaryViewModel
                {
                    Id = salary.Id,
                    Source = salary.Name,
                    IncomeDay = salary.IncomeDay,
                    Income = salary.Value,
                    AnnualIncome = salary.Value * 12,
                    Note = salary.Note
                };

                result.Salaries.Add(viewModel);
            }

            result.TotalIncome = result.Salaries.Sum(x => x.Income.Value);
            result.TotalAnnualIncome = result.TotalIncome * 12;
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);
            return result;
        }

        public static SalarySummaryListViewModel GetSalarySummaryByUser(string username)
        {
            Entities entities = new Entities();
            var salaries = entities.Incomes.Where(x => x.Username.Equals(username)
                                                && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.Name).ToList();
            SalarySummaryListViewModel result = new SalarySummaryListViewModel();
            foreach (var salary in salaries)
            {
                SalarySummaryViewModel viewModel = new SalarySummaryViewModel
                {
                    Source = salary.Name,
                    IncomeDay = salary.IncomeDay.Value,
                    Income = salary.Value,
                    AnnualIncome = salary.Value * 12,
                    Note = salary.Note
                };

                result.Salaries.Add(viewModel);
            }

            result.TotalIncome = result.Salaries.Sum(x => x.Income);
            result.TotalAnnualIncome = result.TotalIncome * 12;
            return result;
        }

        public static SalaryUpdateViewModel GetSalaryById(int id)
        {
            Entities entities = new Entities();
            Incomes salary = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            SalaryUpdateViewModel model = new SalaryUpdateViewModel
            {
                Id = salary.Id,
                Income = salary.Value,
                IncomeDay = salary.IncomeDay,
                Note = salary.Note,
                Source = salary.Name
            };
            return model;
        }

        public static int CreateSalary(SalaryCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Incomes salary = new Incomes();
            salary.Name = model.Source;
            salary.IncomeDay = model.IncomeDay.Value;
            salary.Value = model.Income.Value;
            salary.Note = model.Note;
            salary.IncomeType = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME;
            salary.StartDate = current;
            salary.CreatedDate = current;
            salary.CreatedBy = Constants.Constants.USER;
            salary.Username = username;

            entities.Incomes.Add(salary);
            return entities.SaveChanges();
        }

        public static int UpdateSalary(SalaryUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Incomes salary = entities.Incomes.Where(x => x.Id == model.Id).FirstOrDefault();
            salary.Name = model.Source;
            salary.IncomeDay = model.IncomeDay.Value;
            salary.Value = model.Income.Value;
            salary.Note = model.Note;

            entities.Incomes.Attach(salary);
            entities.Entry(salary).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }

        public static int DeleteSalary(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Incomes salary = entities.Incomes.Where(x => x.Id == id).FirstOrDefault();
            salary.DisabledDate = current;
            salary.DisabledBy = Constants.Constants.USER;
            entities.Incomes.Attach(salary);
            entities.Entry(salary).State = System.Data.Entity.EntityState.Modified;
            return entities.SaveChanges();
        }
    }
}