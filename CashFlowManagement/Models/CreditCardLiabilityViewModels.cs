using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class CreditCardLiabilityCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên ngân hàng")]
        [Display(Name = "Tên ngân hàng")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Nhập giá trị đã dùng")]
        [Display(Name = "Giá trị đã dùng")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double? Value { get; set; }

        [Required(ErrorMessage = "Nhập chi phí lãi")]
        [Display(Name = "Chi phí lãi")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Range(1, 100, ErrorMessage = "Chi phí lãi phải lớn hơn 1 và nhỏ hơn hoặc bằng 100%")]
        public double? InterestRate { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class CreditCardLiabilityUpdateViewModel: CreditCardLiabilityCreateViewModel
    {
        public int Id { get; set; }
    }

    public class CreditCardLiabilityViewModel
    {
        public int Id { get; set; }
        public string Source { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double InterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyPayment { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualPayment { get; set; }

        public string Note { get; set; }
    }   

    public class CreditCardLiabilityListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualPayment { get; set; }
        public bool IsInitialized { get; set; }
        public List<CreditCardLiabilityViewModel> Liabilities { get; set; }
        public CreditCardLiabilityListViewModel()
        {
            Liabilities = new List<CreditCardLiabilityViewModel>();
        }
    }

    public class CreditCardLiabilitySummaryViewModel: CreditCardLiabilityViewModel
    {

    }

    public class CreditCardLiabilitySummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualPayment { get; set; }
        public List<CreditCardLiabilitySummaryViewModel> Liabilities { get; set; }
        public CreditCardLiabilitySummaryListViewModel()
        {
            Liabilities = new List<CreditCardLiabilitySummaryViewModel>();
        }
    }
}