using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class FinancialStatusViewModel
    {
        public double FinancialFreedom { get; set; }
        public double MonthlyCashflow { get; set; }
        public double PassiveIncome { get; set; }

        public double SalaryIncome { get; set; }
        public double RealEstateIncome { get; set; }
        public double BusinessIncome { get; set; }
        public double InterestIncome { get; set; }
        public double DividendIncome { get; set; }

        public double HomeMortgage { get; set; }
        public double CarPayment { get; set; }
        public double CreditCard { get; set; }
        public double BusinessLoanExpenses { get; set; }
        public double StockExpenses { get; set; }
        public double OtherLoanExpenses { get; set; }
        public double FamilyExpenses { get; set; }
        public double OtherExpenses { get; set; }

        public double AvailableMoney { get; set; }
        public double RealEstateValue { get; set; }
        public double BusinessValue { get; set; }
        public double BankDepositValue { get; set; }
        public double StockValue { get; set; }
        public double InsuranceValue { get; set; }

        public double HomeMortgageLiability { get; set; }
        public double CarLoan { get; set; }
        public double CreditCardLiability { get; set; }
        public double BusinessLoan { get; set; }    
        public double StockLoan { get; set; }
        public double OtherLoans { get; set; }

        public double TotalIncomes { get; set; }
        public double TotalExpenses { get; set; }
        public double TotalAssets { get; set; }
        public double TotalLiabilities { get; set; }
        public double Equipty { get; set; }

        public bool CompleteInitialization { get; set; }
    }
}