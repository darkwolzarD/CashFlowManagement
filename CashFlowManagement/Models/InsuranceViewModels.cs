using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class InsuranceCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên bản hiểm")]
        [Display(Name = "Tên bảo hiểm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nhập giá trị thụ hưởng")]
        [Display(Name = "Giá trị thụ hưởng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Value { get; set; }

        [Required(ErrorMessage = "Chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Nhập chi hàng tháng")]
        [Display(Name = "Chi hàng tháng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Expense { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class InsuranceUpdateViewModel : InsuranceCreateViewModel
    {
        public int Id { get; set; }
    }

    public class InsuranceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double YieldRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public string PaymentPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Expense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }

        public string Note { get; set; }

        public int RowSpan { get; set; }
    }

    public class InsuranceListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalTotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalRemainedValue { get; set; }
        public bool IsInitialized { get; set; }

        public List<InsuranceViewModel> Insurances { get; set; }

        public InsuranceListViewModel()
        {
            Insurances = new List<InsuranceViewModel>();
        }
    }

    public class InsuranceSummaryViewModel
    {
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double YieldRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public string PaymentPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Expense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }

        public string Note { get; set; }

        public int RowSpan { get; set; }
    }

    public class InsuranceSummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalTotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalRemainedValue { get; set; }

        public List<InsuranceSummaryViewModel> InsuranceSummaries { get; set; }

        public InsuranceSummaryListViewModel()
        {
            InsuranceSummaries = new List<InsuranceSummaryViewModel>();
        }
    }
}