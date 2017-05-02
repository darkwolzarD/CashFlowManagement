using CashFlowManagement.EntityModel;
using CashFlowManagement.Utilities;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class FinancialStatusQueries
    {
        public static FinancialStatusViewModel GetFinancialStatusByUser(string username)
        {
            FinancialStatusViewModel result = new FinancialStatusViewModel();
            Entities entities = new Entities();

            result.SalaryIncome = entities.Incomes.Where(x => x.Username.Equals(username)
                                                         && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.RealEstateIncome = entities.Incomes.Where(x => x.Username.Equals(username)
                                                         && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.REAL_ESTATE_INCOME
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.BusinessIncome = entities.Incomes.Where(x => x.Username.Equals(username)
                                                         && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.BUSINESS_INCOME
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.InterestIncome = entities.Incomes.Where(x => x.Username.Equals(username)
                                                         && x.IncomeType == (int)Constants.Constants.INCOME_TYPE.BANK_DEPOSIT_INCOME
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.FamilyExpenses = entities.Expenses.Where(x => x.Username.Equals(username)
                                                         && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.FAMILY
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.OtherExpenses = entities.Expenses.Where(x => x.Username.Equals(username)
                                                         && x.ExpenseType == (int)Constants.Constants.EXPENSE_TYPE.OTHERS
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.AvailableMoney = entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.RealEstateValue = entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.BusinessValue = entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BUSINESS
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.BankDepositValue = entities.Assets.Where(x => x.Username.Equals(username)
                                                         && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.HomeMortgageLiability = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE
                                                         && !x.ParentLiabilityId.HasValue && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            var carLiabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                         && !x.DisabledDate.HasValue);
            foreach (var carLiability in carLiabilities)
            {
                result.CarPayment += carLiability.Value / FormatUtility.CalculateTimePeriod(carLiability.StartDate.Value, carLiability.EndDate.Value) + carLiability.Value * carLiability.InterestRate / 1200;
            }

            var creditCardLiabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                         && !x.DisabledDate.HasValue);
            foreach (var creditCarLiability in creditCardLiabilities)
            {
                result.CreditCard += creditCarLiability.Value / FormatUtility.CalculateTimePeriod(creditCarLiability.StartDate.Value, creditCarLiability.EndDate.Value) + creditCarLiability.Value * creditCarLiability.InterestRate / 1200;
            }

            var homeLiabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE
                                                         && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
            foreach (var homeLiability in homeLiabilities)
            {
                result.HomeMortgage += LiabilityQueries.GetCurrentMonthlyPayment(homeLiability.Id);
            }

            var businessLiabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS
                                                         && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
            foreach (var businessLiability in businessLiabilities)
            {
                result.BusinessLoanExpenses += LiabilityQueries.GetCurrentMonthlyPayment(businessLiability.Id);
            }

            var otherLiabilities = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                         && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue);
            foreach (var otherLiability in otherLiabilities)
            {
                result.OtherLoanExpenses += otherLiability.Value / FormatUtility.CalculateTimePeriod(otherLiability.StartDate.Value, otherLiability.EndDate.Value) + otherLiability.Value * otherLiability.InterestRate / 1200;
            }

            result.CreditCardLiability = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.CarLoan = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.CAR
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.BusinessLoan = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS
                                                         && !x.DisabledDate.HasValue && !x.ParentLiabilityId.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.OtherLoans = entities.Liabilities.Where(x => x.Username.Equals(username)
                                                         && x.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.OTHERS
                                                         && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            result.TotalIncomes = result.SalaryIncome + result.RealEstateIncome + result.BusinessIncome + result.InterestIncome + result.DividendIncome;
            result.TotalAssets = result.RealEstateValue + result.BusinessValue + result.BankDepositValue + result.StockValue;
            result.TotalExpenses = result.HomeMortgage + result.CarPayment + result.CreditCard + result.BusinessLoanExpenses + result.StockExpenses + result.OtherExpenses + result.FamilyExpenses + result.OtherExpenses;
            result.TotalLiabilities = result.HomeMortgageLiability + result.CarLoan + result.CreditCardLiability + result.BusinessLoan + result.StockLoan + result.OtherLoans;
            result.Equipty = result.TotalAssets - result.TotalLiabilities;

            result.MonthlyCashflow = result.TotalIncomes - result.TotalExpenses;
            result.PassiveIncome = result.BusinessIncome + result.RealEstateIncome + result.InterestIncome + result.DividendIncome;
            result.FinancialFreedom = result.PassiveIncome / result.TotalExpenses * 100;

            return result;
        }
    }
}