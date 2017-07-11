using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashFlowManagement.Models
{
    public class OtherAssetCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên tài sản")]
        [Display(Name = "Tên tài sản")]
        public string Name { get; set; }

        //[Display(Name = "Ngày mua bất động sản")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? BuyDate { get; set; }

        [Required(ErrorMessage = "Nhập giá trị tài sản")]
        [Display(Name = "Giá trị tài sản")]
        public double? Value { get; set; }

        [Display(Name = "Thu nhập hàng tháng")]
        public double? Income { get; set; }
        public OtherAssetLiabilityListCreateViewModel Liabilities { get; set; }

        [Display(Name = "Bạn có vay khoản nợ nào để mua kinh doanh không?")]
        public bool IsInDebt { get; set; }

        public OtherAssetCreateViewModel()
        {
            Liabilities = new OtherAssetLiabilityListCreateViewModel();
        }
    }

    public class OtherAssetUpdateViewModel : OtherAssetCreateViewModel
    {
        public int Id { get; set; }
    }

    public class OtherAssetViewModel
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
        public List<OtherAssetLiabilityViewModel> Liabilities { get; set; }

        public OtherAssetViewModel()
        {
            Liabilities = new List<OtherAssetLiabilityViewModel>();
        }
    }

    public class OtherAssetListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalRentYield { get; set; }
        public List<OtherAssetViewModel> Assets { get; set; }

        public OtherAssetListViewModel()
        {
            Assets = new List<Models.OtherAssetViewModel>();
        }
    }

    public class OtherAssetSummaryViewModel
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

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }
    }

    public class OtherAssetSummaryListViewModel
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

        public List<OtherAssetSummaryViewModel> OtherAssetSummaries { get; set; }
        public OtherAssetSummaryListViewModel()
        {
            OtherAssetSummaries = new List<OtherAssetSummaryViewModel>();
        }
    }
}