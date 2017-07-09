using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class OtherAssetLiabilityCreateViewModel : LiabilityCreateViewModel { }

    public class OtherAssetLiabilityUpdateViewModel : LiabilityUpdateViewModel { }

    public class OtherAssetLiabilityViewModel : LiabilityViewModel { }

    public class OtherAssetLiabilityListViewModel
    {
        public List<OtherAssetLiabilityViewModel> Liabilities { get; set; }
        public OtherAssetLiabilityListViewModel()
        {
            Liabilities = new List<OtherAssetLiabilityViewModel>();
        }
    }

    public class OtherAssetLiabilityListCreateViewModel
    {
        public List<OtherAssetLiabilityCreateViewModel> Liabilities { get; set; }
        public OtherAssetLiabilityListCreateViewModel()
        {
            Liabilities = new List<OtherAssetLiabilityCreateViewModel>();
        }
    }
}