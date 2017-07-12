using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class AvailableMoneyCreateViewModel
    {
        [Display(Name = "Tiền mặt có sẵn")]
        [Required(ErrorMessage = "Nhập tiền mặt có sẵn")]
        public double? AvailableMoney { get; set; }

        public bool Editable { get; set; }
    }
}