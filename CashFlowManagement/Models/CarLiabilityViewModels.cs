using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static CashFlowManagement.Models.BaseLiabilityModels;

namespace CashFlowManagement.Models
{
    public class CarLiabilityCreateViewModel : LiabilityCreateViewModel {
        [Required(ErrorMessage = "Nhập tổng giá trị")]
        [Display(Name = "Tổng giá trị")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double? LiabilityValue { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class CarLiabilityUpdateViewModel : CarLiabilityCreateViewModel { }

    public class CarLiabilityViewModel : LiabilityViewModel {

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double LiabilityValue { get; set; }
        public string Note { get; set; }
    }

    public class CarLiabilityListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalOriginalValue { get; set; }

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
        public List<CarLiabilityViewModel> Liabilities { get; set; }
        public CarLiabilityListViewModel()
        {
            Liabilities = new List<CarLiabilityViewModel>();
        }
    }

    public class CarLiabilityListCreateViewModel
    {
        public List<CarLiabilityCreateViewModel> Liabilities { get; set; }
        public CarLiabilityListCreateViewModel()
        {
            Liabilities = new List<CarLiabilityCreateViewModel>();
        }
    }

    public class CarLiabilitySummaryViewModel : CarLiabilityViewModel
    {

    }

    public class CarLiabilitySummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalOriginalValue { get; set; }

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
        public List<CarLiabilitySummaryViewModel> Liabilities { get; set; }
        public CarLiabilitySummaryListViewModel()
        {
            Liabilities = new List<CarLiabilitySummaryViewModel>();
        }
    }
}