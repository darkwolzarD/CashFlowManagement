using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class BusinessLiabilityCreateViewModel : LiabilityCreateViewModel { }

    public class BusinessLiabilityUpdateViewModel : LiabilityUpdateViewModel { }

    public class BusinessLiabilityViewModel : LiabilityViewModel { }

    public class BusinessLiabilityListViewModel
    {
        public List<BusinessLiabilityViewModel> Liabilities { get; set; }
        public BusinessLiabilityListViewModel()
        {
            Liabilities = new List<BusinessLiabilityViewModel>();
        }
    }

    public class BusinessLiabilityListCreateViewModel
    {
        public List<BusinessLiabilityCreateViewModel> Liabilities { get; set; }
        public BusinessLiabilityListCreateViewModel()
        {
            Liabilities = new List<BusinessLiabilityCreateViewModel>();
        }
    }
}