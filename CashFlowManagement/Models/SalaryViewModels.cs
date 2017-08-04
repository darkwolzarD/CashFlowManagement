using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class SalaryCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên nguồn thu!")]
        [Display(Name = "Tên nguồn thu")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày nhận lương!")]
        [Display(Name = "Ngày nhận lương hàng tháng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? IncomeDay { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thu nhập từ lương!")]
        [Display(Name = "Thu nhập hàng tháng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Income { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class SalaryUpdateViewModel : SalaryCreateViewModel
    {
        public int Id { get; set; }
    }

    public class SalaryViewModel : SalaryUpdateViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }
    }

    public class SalarySummaryViewModel
    {
        public string Source { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int IncomeDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Income { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualIncome { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class SalaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        public bool IsInitialized { get; set; }

        public List<SalaryViewModel> Salaries { get; set; }
        public SalaryListViewModel()
        {
            Salaries = new List<SalaryViewModel>();
        }
    }

    public class SalarySummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualIncome { get; set; }

        public List<SalarySummaryViewModel> Salaries { get; set; }
        public SalarySummaryListViewModel()
        {
            Salaries = new List<SalarySummaryViewModel>();
        }
    }
}