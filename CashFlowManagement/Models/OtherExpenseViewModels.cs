using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class OtherExpenseCreateViewModel
    {
        [Required(ErrorMessage = "Nhập tên nguồn chi tiêu")]
        [Display(Name = "Tên nguồn chi tiêu")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Nhập ngày chi tiêu")]
        [Display(Name = "Ngày chi tiêu hàng tháng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? ExpenseDay { get; set; }

        [Required(ErrorMessage = "Nhập chi tiêu hàng tháng")]
        [Display(Name = "Chi tiêu hàng tháng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Expense { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class OtherExpenseUpdateViewModel : OtherExpenseCreateViewModel
    {
        public int Id { get; set; }
    }

    public class OtherExpenseViewModel : OtherExpenseUpdateViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualExpense { get; set; }
    }

    public class OtherExpenseListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualExpense { get; set; }

        public List<OtherExpenseViewModel> Expenses { get; set; }
        public OtherExpenseListViewModel()
        {
            Expenses = new List<OtherExpenseViewModel>();
        }
    }
}