using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class RealEstateLiabilityCreateViewModel
    {
        public string Source { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        public string InterestType { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double InterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime EndDate { get; set; }
    }

    public class RealEstateLiabilityViewModel: RealEstateLiabilityCreateViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PaymentPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyOriginalPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }
    }
}