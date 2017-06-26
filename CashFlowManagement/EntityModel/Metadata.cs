using System;
using System.ComponentModel.DataAnnotations;

namespace CashFlowManagement.EntityModel
{
    public class IncomeMetadata
    {
        [Required(ErrorMessage = "Nhập nguồn thu nhập")]
        public string Name;

        [Required(ErrorMessage = "Nhập số thu hàng tháng")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double Value;

        [Required(ErrorMessage = "Chọn tháng bắt đầu")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate;
    }

    public class ExpenseMetadata
    {
        [Required(ErrorMessage = "Nhập nguồn thu nhập")]
        public string Name;

        [Required(ErrorMessage = "Nhập số thu hàng tháng")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double Value;

        [Required(ErrorMessage = "Chọn tháng bắt đầu")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate;
    }
}