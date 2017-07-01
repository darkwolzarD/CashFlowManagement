using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class RealEstateLiabilityCreateViewModel
    {
        [Display(Name = "Nguồn vay nợ")]
        public string Source { get; set; }

        [Display(Name = "Giá trị nợ")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [Display(Name = "Loại lãi suất")]
        public string InterestType { get; set; }

        [Display(Name = "Lãi suất vay")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double InterestRate { get; set; }

        [Display(Name = "Ngày vay nợ")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày kết thúc nợ")]
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
        public string Status { get; set; }
        public string StatusCode { get; set; }
    }
}