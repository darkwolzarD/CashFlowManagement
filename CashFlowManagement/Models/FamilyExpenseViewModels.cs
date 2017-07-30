using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class FamilyExpenseCreateViewModel
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

    public class FamilyExpenseUpdateViewModel : FamilyExpenseCreateViewModel
    {
        public int Id { get; set; }
    }

    public class FamilyExpenseViewModel : FamilyExpenseUpdateViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualExpense { get; set; }
    }

    public class FamilyExpenseListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualExpense { get; set; }
        public bool IsInitialized { get; set; }
        public List<FamilyExpenseViewModel> Expenses { get; set; }
        public FamilyExpenseListViewModel()
        {
            Expenses = new List<FamilyExpenseViewModel>();
        }
    }

    public class FamilyExpenseSummaryViewModel : FamilyExpenseViewModel
    {

    }

    public class FamilyExpenseSummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualExpense { get; set; }

        public List<FamilyExpenseSummaryViewModel> Expenses { get; set; }
        public FamilyExpenseSummaryListViewModel()
        {
            Expenses = new List<FamilyExpenseSummaryViewModel>();
        }
    }
}