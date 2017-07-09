using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CashFlowManagement.Queries.BusinessLiabilityQueries;

namespace CashFlowManagement.Queries
{
    public class InsuranceQueries
    {
        public static InsuranceListViewModel GetInsuranceByUser(string username)
        {
            Entities entities = new Entities();
            var insurances = entities.Assets.Include("Expenses").Where(x => x.Username.Equals(username) 
                                                && x.AssetType == (int)Constants.Constants.ASSET_TYPE.INSURANCE 
                                                && !x.DisabledDate.HasValue).OrderBy(x => x.AssetName).ToList();
            InsuranceListViewModel result = new InsuranceListViewModel();
            foreach (var insurance in insurances)
            {
                var expense = insurance.Expenses1.FirstOrDefault();
                InsuranceViewModel viewModel = new InsuranceViewModel
                {
                    Id = insurance.Id,
                    Name = insurance.AssetName,
                    Value = insurance.Value,
                    StartDate = insurance.StartDate.Value,
                    EndDate = insurance.EndDate.Value,
                    PaymentPeriod = Helper.CalculateTimePeriod(insurance.StartDate.Value, insurance.EndDate.Value),
                    Expense = expense.Value,
                    AnnualExpense = expense.Value * 12,
                    Note = insurance.Note
                };

                viewModel.TotalExpense = viewModel.PaymentPeriod * viewModel.Expense;
                int currentPeriod = Helper.CalculateTimePeriod(viewModel.StartDate, DateTime.Now);
                viewModel.RemainedValue = viewModel.TotalExpense - viewModel.Expense * currentPeriod;

                result.Insurances.Add(viewModel);
            }

            result.TotalValue = result.Insurances.Sum(x => x.Value);
            result.TotalTotalExpense = result.Insurances.Sum(x => x.TotalExpense);
            result.TotalExpense = result.Insurances.Sum(x => x.Expense);
            result.TotalAnnualExpense = result.Insurances.Sum(x => x.AnnualExpense);
            result.TotalRemainedValue = result.Insurances.Sum(x => x.RemainedValue);

            return result;
        }

        public static InsuranceUpdateViewModel GetInsuranceById(int id)
        {
            Entities entities = new Entities();
            Assets insurance = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            var expense = insurance.Expenses1.FirstOrDefault();
            InsuranceUpdateViewModel model = new InsuranceUpdateViewModel
            {
                Id = insurance.Id,
                Value = insurance.Value,
                Name = insurance.AssetName,
                StartDate = insurance.StartDate.Value,
                EndDate = insurance.EndDate.Value,
                Expense = expense.Value,
                Note = insurance.Note
            };
            return model;
        }

        public static int CreateInsurance(InsuranceCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets insurance = new Assets();
            insurance.AssetName = model.Name;
            insurance.Value = model.Value.Value;
            insurance.StartDate = model.StartDate.Value;
            insurance.EndDate = model.EndDate.Value;
            insurance.Note = model.Note;
            insurance.CreatedDate = current;
            insurance.CreatedBy = Constants.Constants.USER;
            insurance.AssetType = (int)Constants.Constants.ASSET_TYPE.INSURANCE;
            insurance.Username = username;

            Expenses expense = new Expenses();
            expense.Name = "Đóng bảo hiểm " + insurance.AssetName;
            expense.Value = model.Expense.Value;
            expense.StartDate = model.StartDate.Value;
            expense.EndDate = model.EndDate.Value;
            expense.CreatedDate = current;
            expense.CreatedBy = Constants.Constants.USER;
            expense.ExpenseType = (int)Constants.Constants.EXPENSE_TYPE.INSURANCE;
            expense.Username = username;

            insurance.Expenses1.Add(expense);
            entities.Assets.Add(insurance);

            return entities.SaveChanges();
        }

        public static int UpdateInsurance(InsuranceUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets insurance = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();
            insurance.AssetName = model.Name;
            insurance.Value = model.Value.Value;
            insurance.StartDate = model.StartDate.Value;
            insurance.EndDate = model.EndDate.Value;
            insurance.Note = model.Note;

            Expenses expense = entities.Expenses.Where(x => x.AssetId == model.Id).FirstOrDefault();
            expense.Name = "Đóng bảo hiểm " + insurance.AssetName;
            expense.Value = model.Expense.Value;
            expense.StartDate = model.StartDate.Value;
            expense.EndDate = model.EndDate.Value;

            entities.Assets.Attach(insurance);
            entities.Entry(insurance).State = System.Data.Entity.EntityState.Modified;

            entities.Expenses.Attach(expense);
            entities.Entry(expense).State = System.Data.Entity.EntityState.Modified;

            return entities.SaveChanges();
        }

        public static int DeleteInsurance(int id)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets insurance = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            insurance.DisabledDate = current;
            insurance.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(insurance);
            entities.Entry(insurance).State = System.Data.Entity.EntityState.Modified;

            Expenses expense = entities.Expenses.Where(x => x.AssetId == id).FirstOrDefault();
            expense.DisabledDate = current;
            expense.DisabledBy = Constants.Constants.USER;
            entities.Expenses.Attach(expense);
            entities.Entry(expense).State = System.Data.Entity.EntityState.Modified;

            return entities.SaveChanges();
        }
    }
}