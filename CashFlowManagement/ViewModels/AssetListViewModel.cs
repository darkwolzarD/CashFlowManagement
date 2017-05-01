using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class AssetListViewModel
    {
        public int Type { get; set; }
        public List<AssetViewModel> List { get; set; }
        public double TotalMonthlyIncome { get; set; }
        public double TotalValue { get; set; }
        public AssetListViewModel()
        {
            List = new List<AssetViewModel>();
        }
    }
}