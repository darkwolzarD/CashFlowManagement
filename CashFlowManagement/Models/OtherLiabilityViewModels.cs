using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class OtherLiabilityCreateViewModel : LiabilityCreateViewModel {
        [Required(ErrorMessage = "Nhập mục tiêu sử dụng")]
        [Display(Name = "Mục tiêu sử dụng")]
        public string Purpose { get; set; }

        public string Note { get; set; }
    }

    public class OtherLiabilityUpdateViewModel : OtherLiabilityCreateViewModel { }

    public class OtherLiabilityViewModel : LiabilityViewModel {
        public string Purpose { get; set; }
        public string Note { get; set; }
    }

    public class OtherLiabilityListViewModel
    {

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
        public List<OtherLiabilityViewModel> Liabilities { get; set; }
        public OtherLiabilityListViewModel()
        {
            Liabilities = new List<OtherLiabilityViewModel>();
        }
    }

    public class OtherLiabilityListCreateViewModel
    {
        public List<OtherLiabilityCreateViewModel> Liabilities { get; set; }
        public OtherLiabilityListCreateViewModel()
        {
            Liabilities = new List<OtherLiabilityCreateViewModel>();
        }
    }
}