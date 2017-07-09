using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class StockLiabilityCreateViewModel : LiabilityCreateViewModel { }

    public class StockLiabilityUpdateViewModel : LiabilityUpdateViewModel { }

    public class StockLiabilityViewModel : LiabilityViewModel { }

    public class StockLiabilityListViewModel
    {
        public List<StockLiabilityViewModel> Liabilities { get; set; }
        public StockLiabilityListViewModel()
        {
            Liabilities = new List<StockLiabilityViewModel>();
        }
    }

    public class StockLiabilityListCreateViewModel
    {
        public List<StockLiabilityCreateViewModel> Liabilities { get; set; }
        public StockLiabilityListCreateViewModel()
        {
            Liabilities = new List<StockLiabilityCreateViewModel>();
        }
    }
}