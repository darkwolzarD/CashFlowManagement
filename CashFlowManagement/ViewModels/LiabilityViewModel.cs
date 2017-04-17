using CashFlowManagement.EntityModel;

namespace CashFlowManagement.ViewModels
{
    public class LiabilityViewModel
    {
        public Liabilities Liability { get; set; }
        public double CurrentInterestRate { get; set; }
        public int TotalPaymentPeriod { get; set; }
        public double MonthlyOriginalPayment { get; set; }
        public double MonthlyInterestPayment { get; set; }
        public double MonthlyPayment { get; set; }
        public double AnnualPayment { get; set; }
        public double RemainedValue { get; set; }

        public LiabilityViewModel()
        {
            Liability = new Liabilities();
        }
    }
}