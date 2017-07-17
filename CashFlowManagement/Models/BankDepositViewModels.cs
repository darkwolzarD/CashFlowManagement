using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class BankDepositCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên tài khoản tiết kiệm")]
        [Display(Name = "Tên tài khoản tiết kiệm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nhập tổng vốn")]
        [Display(Name = "Tổng vốn")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Value { get; set; }

        [Required(ErrorMessage = "Chọn ngày gửi")]
        [Display(Name = "Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Nhập kỳ hạn")]
        [Display(Name = "Kỳ hạn")]
        public int PaymentPeriod { get; set; }

        [Required(ErrorMessage = "Chọn ngày đáo hạn")]
        [Display(Name = "Ngày đáo hạn")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Nhập lãi suất tiền gửi")]
        [Display(Name = "Lãi suất tiền gửi")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double? InterestRate { get; set; }

        [Required(ErrorMessage = "Chọn lãi suất năm hoặc tháng")]
        [Display(Name = "Lãi suất áp dụng")]
        public int InterestRatePerX { get; set; }

        [Required(ErrorMessage = "Chọn kiểu nhận lãi")]
        [Display(Name = "Kiểu nhận lãi")]
        public int InterestObtainWay { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class BankDepositUpdateViewModel : BankDepositCreateViewModel
    {
        public int Id { get; set; }
    }

    public class BankDepositViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double PaymentPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Income { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double? InterestRate { get; set; }

        public string InterestRatePerX { get; set; }

        public string InterestObtainWay { get; set; }

        public string Note { get; set; }
    }

    public class BankDepositListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        public List<BankDepositViewModel> BankDeposits { get; set; }

        public BankDepositListViewModel()
        {
            BankDeposits = new List<BankDepositViewModel>();
        }
    }

    public class BankDepositSummaryViewModel
    {
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double PaymentPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Income { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double? InterestRate { get; set; }

        public string InterestRatePerX { get; set; }

        public string InterestObtainWay { get; set; }

        public string Note { get; set; }
    }

    public class BankDepositSummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:02}")]
        public double TotalInterestRate { get; set; }

        public List<BankDepositSummaryViewModel> BankDepositSummaries { get; set; }

        public BankDepositSummaryListViewModel()
        {
            BankDepositSummaries = new List<BankDepositSummaryViewModel>();
        }
    }
}