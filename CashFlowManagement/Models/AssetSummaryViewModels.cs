using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class AssetSummaryViewModel
    {
        public RealEstateSummaryListViewModel RealEstates { get; set; }
        public BusinessListViewModel Businesses { get; set; }
        public BankDepositListViewModel BankDeposits { get; set; }
        public StockListViewModel Stocks { get; set; }
        public InsuranceListViewModel Insurances { get; set; }
        public OtherAssetListViewModel OtherAssets { get; set; }

        public AssetSummaryViewModel()
        {
            RealEstates = new RealEstateSummaryListViewModel();
            Businesses = new BusinessListViewModel();
            BankDeposits = new BankDepositListViewModel();
            Stocks = new StockListViewModel();
            Insurances = new InsuranceListViewModel();
            OtherAssets = new OtherAssetListViewModel();
        }
    }
}