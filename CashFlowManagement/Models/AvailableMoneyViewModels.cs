using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class AvailableMoneyViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Tiền mặt có sẵn")]
        [Required(ErrorMessage = "Nhập tiền mặt có sẵn")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? AvailableMoney { get; set; }
        public bool IsInitialized { get; set; }
    }

    public class AvailableMoneyCreateViewModel: AvailableMoneyViewModel
    {
        
    }

    public class AvailableMoneySummaryViewModel : AvailableMoneyViewModel
    {

    }
}