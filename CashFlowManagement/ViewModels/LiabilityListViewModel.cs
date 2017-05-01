using System.Collections.Generic;

namespace CashFlowManagement.ViewModels
{
    public class LiabilityListViewModel
    {
        public List<LiabilityViewModel> List { get; set; }
        public int Type { get; set; }
        public double TotalOriginalValue { get; set; }
        public double TotalLiabilityValue { get; set; }
        public double AvarageInterestRate { get; set; }
        public double TotalMonthlyOriginalPayment { get; set; }
        public double TotalMonthlyInterestPayment { get; set; }
        public double TotalMonthlyPayment { get; set; }
        public double RemainedValue { get; set; }
        public LiabilityListViewModel()
        {
            List = new List<LiabilityViewModel>();
        }
    }
}