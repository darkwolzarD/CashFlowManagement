using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels.FinancialStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Utilities
{
    public class FinancialStatusProcessing
    {
        public static FinancialStatusViewModel GetFinancialStatusByUser(string username)
        {
            FinancialStatusViewModel result = new FinancialStatusViewModel();

            //Get salary incomes
            List<Salary> salaryList = SalaryQueries.GetSalaryByUser(username);
            foreach (var salary in salaryList)
            {
                result.SalaryIncome += salary.Income;
            }

            //Get real estate incomes, total value
            List<RealEstateIncomes> realEstateList = RealEstateQueries.GetRealEstateByUser(username);
            foreach (var realEstate in realEstateList)
            {
                result.RealEstateIncome += realEstate.Income;
                result.RealEstateValue += realEstate.OriginalValue;
                foreach (var loan in realEstate.Loans.Where(x => !x.ParentLoanId.HasValue && !x.DisabledDate.HasValue))
                {
                    result.BankLoan += loan.MortgageValue;
                }
            }

            //Get business incomes, total value
            List<BusinessIncomes> businessList = BusinessQueries.GetBusinessByUser(username);
            foreach (var business in businessList) 
            {
                result.BusinessIncome += business.Income;
                result.BusinessValue += business.CapitalValue;
            }

            //Get interest incomes, total value
            List<BankDepositIncomes> bankDepositList = BankDepositQueries.GetBankDepositByUser(username);
            foreach (var deposit in bankDepositList)
            {
                result.InterestIncome += deposit.CapitalValue * deposit.InterestYield / 100;
                result.BankDepositValue += deposit.CapitalValue;
            }

            //Get diviend incomes


            //Get bank loan expenses
            result.BankLoanExpenses = LoanProcessing.GetCurrentMonthlyPaymentByUser(username);

            result.TotalIncomes = result.SalaryIncome + result.RealEstateIncome + result.BusinessIncome + result.InterestIncome + result.DividendIncome;
            result.TotalAssets = result.RealEstateValue + result.BusinessValue + result.BankDepositValue + result.StockValue;
            result.TotalExpenses = result.HomeMortgage + result.CarPayment + result.CreditCard + result.BankLoanExpenses + result.FamilyExpenses + result.OtherExpenses;
            result.TotalLiabilities = result.HomeMortgageLiability + result.CarLoan + result.CreditCardLiability + result.BankLoan + result.OtherLoans;
            result.MonthlyCashflow = result.TotalIncomes - result.TotalExpenses;
            result.PassiveIncome = result.RealEstateIncome + result.BusinessIncome + result.InterestIncome + result.DividendIncome;
            result.FinancialFreedom = result.TotalExpenses > 0 ? result.PassiveIncome / result.TotalExpenses * 100 : 0;

            return result;
        }
    }
}