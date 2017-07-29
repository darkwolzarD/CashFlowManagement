using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class BusinessCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên kinh doanh")]
        [Display(Name = "Tên kinh doanh")]
        public string Name { get; set; }

        //[Display(Name = "Ngày mua bất động sản")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? BuyDate { get; set; }

        [Required(ErrorMessage = "Nhập giá trị góp vốn")]
        [Display(Name = "Giá trị góp vốn")]
        public double? Value { get; set; }

        [Display(Name = "Thu nhập hàng tháng")]
        public double? Income { get; set; }
        public BusinessLiabilityListCreateViewModel Liabilities { get; set; }

        [Display(Name = "Bạn có vay khoản nợ nào để mua kinh doanh không?")]
        public bool IsInDebt { get; set; }

        public BusinessCreateViewModel()
        {
            Liabilities = new BusinessLiabilityListCreateViewModel();
        }
    }

    public class BusinessUpdateViewModel : BusinessCreateViewModel
    {
        public int Id { get; set; }
    }

    public class BusinessViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Income { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double RentYield { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalLiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalOriginalPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalRemainedValue { get; set; }

        public int RowSpan { get; set; }
        public List<BusinessLiabilityViewModel> Liabilities { get; set; }

        public BusinessViewModel()
        {
            Liabilities = new List<BusinessLiabilityViewModel>();
        }
    }

    public class BusinessListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalRentYield { get; set; }
        public List<BusinessViewModel> Businesses { get; set; }

        public BusinessListViewModel()
        {
            Businesses = new List<Models.BusinessViewModel>();
        }
    }

    public class BusinessSummaryViewModel
    {
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Income { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double RentYield { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double LiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double InterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public string InterestRatePerX { get; set; }

        public double OriginalInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }
    }

    public class BusinessSummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalRentYield { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalLiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalRemainedValue { get; set; }

        public List<BusinessSummaryViewModel> BusinessSummaries { get; set; }
        public BusinessSummaryListViewModel()
        {
            BusinessSummaries = new List<BusinessSummaryViewModel>();
        }
    }
}