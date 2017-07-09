using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class RealEstateLiabilityCreateViewModel: LiabilityCreateViewModel { }

    public class RealEstateLiabilityUpdateViewModel : LiabilityUpdateViewModel { }

    public class RealEstateLiabilityViewModel: LiabilityViewModel { }

    public class RealEstateLiabilityListViewModel
    {
        public List<RealEstateLiabilityViewModel> Liabilities { get; set; }
        public RealEstateLiabilityListViewModel()
        {
            Liabilities = new List<RealEstateLiabilityViewModel>();
        }
    }

    public class RealEstateLiabilityListCreateViewModel
    {
        public List<RealEstateLiabilityCreateViewModel> Liabilities { get; set; }
        public RealEstateLiabilityListCreateViewModel()
        {
            Liabilities = new List<RealEstateLiabilityCreateViewModel>();
        }
    }
}