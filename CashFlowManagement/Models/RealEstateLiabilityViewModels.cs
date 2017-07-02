using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class RealEstateLiabilityCreateViewModel
    {
        [Required(ErrorMessage = "Nhập nguồn vay nợ")]
        [Display(Name = "Nguồn vay nợ")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Nhập giá trị nợ")]
        [Display(Name = "Giá trị nợ")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double? Value { get; set; }

        [Required(ErrorMessage = "Chọn loại lãi suất")]
        [Display(Name = "Loại lãi suất")]
        public int InterestType { get; set; }

        [Required(ErrorMessage = "Nhập lãi suất vay")]
        [Display(Name = "Lãi suất vay")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public double? InterestRate { get; set; }

        [Required(ErrorMessage = "Chọn ngày vay nợ")]
        [Display(Name = "Ngày vay nợ")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Chọn ngày kết thúc nợ")]
        [Display(Name = "Ngày kết thúc nợ")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
    }

    public class RealEstateLiabilityViewModel
    {
        public string Source { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Value { get; set; }
        public string InterestType { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double? InterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? EndDate { get; set; }
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