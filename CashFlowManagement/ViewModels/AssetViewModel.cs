using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class AssetViewModel
    {
        public Assets Asset { get; set; }
        public Incomes Income { get; set; }
        public Liabilities Liability { get; set; }
        public StockTransactions Transaction { get; set; }
        public double BuyAmount { get; set; }
        public double SellAmount { get; set; }
        public int RemainedStock { get; set; }
        public double CurrentAvailableMoney { get; set; }
        public LiabilityListViewModel LiabilityList { get; set; }
        public double TotalMortgageValue { get; set; }
        public double AverageInterestRate { get; set; }
        public double TotalOriginalPayment { get; set; }
        public double TotalInterestPayment { get; set; }
        public double TotalMonthlyPayment { get; set; }
        public double TotalAnnualPayment { get; set; }
        public double TotalRemainingValue { get; set; }

        public AssetViewModel()
        {
            this.LiabilityList = new LiabilityListViewModel();
        }
    }
}