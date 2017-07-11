using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class AssetSummaryViewModel
    {
        public RealEstateSummaryListViewModel RealEstates { get; set; }
        public BusinessSummaryListViewModel Businesses { get; set; }
        public BankDepositSummaryListViewModel BankDeposits { get; set; }
        public StockSummaryListViewModel Stocks { get; set; }
        public InsuranceSummaryListViewModel Insurances { get; set; }
        public OtherAssetSummaryListViewModel OtherAssets { get; set; }

        public AssetSummaryViewModel()
        {
            RealEstates = new RealEstateSummaryListViewModel();
            Businesses = new BusinessSummaryListViewModel();
            BankDeposits = new BankDepositSummaryListViewModel();
            Stocks = new StockSummaryListViewModel();
            Insurances = new InsuranceSummaryListViewModel();
            OtherAssets = new OtherAssetSummaryListViewModel();
        }
    }

    public class LiabilitySummaryViewModel
    {
        public CarLiabilitySummaryListViewModel CarLiabilities { get; set; }
        public CreditCardLiabilitySummaryListViewModel CreditCardLiabilities { get; set; }
        public OtherLiabilitySummaryListViewModel OtherLiabilities { get; set; }

        public LiabilitySummaryViewModel()
        {
            CarLiabilities = new CarLiabilitySummaryListViewModel();
            CreditCardLiabilities = new CreditCardLiabilitySummaryListViewModel();
            OtherLiabilities = new OtherLiabilitySummaryListViewModel();
        }
    }


    public class ExpenseSummaryViewModel
    {
        public FamilyExpenseSummaryListViewModel FamilyExpenses { get; set; }
        public OtherExpenseSummaryListViewModel OtherExpenses { get; set; }

        public ExpenseSummaryViewModel()
        {
            FamilyExpenses = new FamilyExpenseSummaryListViewModel();
            OtherExpenses = new OtherExpenseSummaryListViewModel();
        }
    }
}