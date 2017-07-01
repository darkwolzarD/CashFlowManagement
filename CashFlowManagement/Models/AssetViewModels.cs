using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class RealEstateCreateViewModel
    {
        [Required]
        [Display(Name = "Tên bất động sản")]
        public string Name { get; set; }

        [Display(Name = "Ngày mua bất động sản")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BuyDate { get; set; }

        [Required]
        [Display(Name = "Giá trị bất động sản")]
        public double? Value { get; set; }

        [Required]
        [Display(Name = "Thu nhập hàng tháng cho thuê")]
        public double? Income { get; set; }
        public List<RealEstateLiabilityCreateViewModel> Liabilities { get; set; }
        
        [Display(Name = "Bạn có vay khoản nợ nào để mua bất động sản này không?")]
        public bool IsInDept { get; set; }

        public RealEstateCreateViewModel()
        {
            Liabilities = new List<RealEstateLiabilityCreateViewModel>();
        }
    }

    public class RealEstateUpdateViewModel : RealEstateCreateViewModel
    {
        public int Id { get; set; }
    }

    public class RealEstateViewModel
    {
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
        public List<RealEstateLiabilityViewModel> Liabilities { get; set; }

        public RealEstateViewModel()
        {
            Liabilities = new List<RealEstateLiabilityViewModel>();
        }
    }

    public class RealEstateListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalRentYield { get; set; }
        public List<RealEstateViewModel> RealEstates { get; set; }

        public RealEstateListViewModel()
        {
            RealEstates = new List<Models.RealEstateViewModel>();
        }
    }
}
