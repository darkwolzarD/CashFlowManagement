using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class AssetListViewModel
    {
        public int Type { get; set; }
        public IQueryable<AssetViewModel> List { get; set; }
    }
}