using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class StockTransactionViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? NumberOfStock { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? SpotRice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? StockValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? ExpectedDividend { get; set; }

        public StockLiabilityListViewModel Liabilities { get; set; }
        public StockTransactionViewModel() {
            Liabilities = new StockLiabilityListViewModel();
        }
    }

    public class StockTransactionListViewModel
    {
        public List<StockTransactionViewModel> Transactions { get; set; }
        public StockTransactionListViewModel()
        {
            Transactions = new List<StockTransactionViewModel>();
        }
    }
}