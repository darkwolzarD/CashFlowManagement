using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class BusinessLiabilityCreateViewModel : LiabilityViewModel { }

    public class BusinessLiabilityUpdateViewModel : LiabilityCreateViewModel { }

    public class BusinessLiabilityViewModel : LiabilityViewModel { }

    public class BusinessLiabilityListViewModel : LiabilityListViewModel { }

    public class BusinessLiabilityListCreateViewModel : LiabilityListCreateViewModel { }
}