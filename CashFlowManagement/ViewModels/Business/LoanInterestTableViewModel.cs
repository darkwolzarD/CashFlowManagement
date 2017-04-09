using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels.Business
{
    public class LoanInterestTableViewModel
    {        
        public double CurrentInterestRatePerYear { get; set; }
        public double RemainingLoan { get; set; }
        public double MonthlyOriginalPayment { get; set; }
        public double MonthlyInterestPayment { get; set; }
        public double MonthlyTotalPayment { get; set; }
        public DateTime CurrentMonth { get; set; }
        public bool Highlight { get; set; }
    }
}