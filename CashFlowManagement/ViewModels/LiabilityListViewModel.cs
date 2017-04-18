using System.Collections.Generic;

namespace CashFlowManagement.ViewModels
{
    public class LiabilityListViewModel
    {
        public List<LiabilityViewModel> List { get; set; }
        public int Type { get; set; }

        public LiabilityListViewModel()
        {
            List = new List<LiabilityViewModel>();
        }
    }
}