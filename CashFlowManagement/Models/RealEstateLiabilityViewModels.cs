using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class RealEstateLiabilityCreateViewModel: LiabilityViewModel { }

    public class RealEstateLiabilityUpdateViewModel : LiabilityCreateViewModel { }

    public class RealEstateLiabilityViewModel: LiabilityViewModel { }

    public class RealEstateLiabilityListViewModel: LiabilityListViewModel { }

    public class RealEstateLiabilityListCreateViewModel: LiabilityListCreateViewModel { }
}