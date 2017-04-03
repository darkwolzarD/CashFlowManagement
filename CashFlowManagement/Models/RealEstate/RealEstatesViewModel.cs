using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models.RealEstate
{
    public class RealEstatesViewModel
    {
        public List<RealEstateIncomes> lst_RealEstateIncomes { get; set; }
        public double dAnnualRentIncomes { get; set; }
        public double dRentYield { get; set; }
        public double dTotalMorgageValue { get; set; }
        public double dAverageInterestRate { get; set; }
        public double dTotalMonthlyIncomes { get; set; }
        public double dTotalAnnualIncome { get; set; }
        public double dTotalRemainingValue { get; set; }

        public List<int> lst_iPaymentMonths { get; set; }
        public List<double> lst_dCurrentMonthlyInterestPayment { get; set; }
        public List<double> lst_dMonthlyTotalPayment { get; set; }
        public List<double> lst_dRemainingValue { get; set; }
    }
}