﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class BaseLiabilityModels
    {
        public class LiabilityCreateViewModel : IValidatableObject
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập nguồn vay nợ!")]
            [Display(Name = "Nguồn vay nợ")]
            public string Source { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập giá trị nợ!")]
            [Display(Name = "Giá trị nợ")]
            [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
            public double? Value { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn loại lãi suất!")]
            [Display(Name = "Loại lãi suất")]
            public int InterestType { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập lãi suất vay!")]
            [Display(Name = "Lãi suất vay")]
            [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
            [Range(0, 100, ErrorMessage = "Lãi suất phải nằm trong khoảng 0% - 100%")]
            public double? InterestRate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu vay!")]
            [Display(Name = "Ngày bắt đầu vay")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime? StartDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập ngày trả hết nợ!")]
            [Display(Name = "Ngày trả hết nợ")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime? EndDate { get; set; }

            [Required(ErrorMessage = "Chọn lãi suất năm hoặc tháng")]
            [Display(Name = "Lãi suất áp dụng")]
            public int InterestRatePerX { get; set; }
            public int AssetId { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (EndDate < StartDate)
                {
                    yield return
                      new ValidationResult(errorMessage: "Ngày bắt đầu vay phải nhỏ hơn ngày trả hết nợ",
                                           memberNames: new[] { "EndDate" });
                }
            }
        }

        public class LiabilityUpdateViewModel : LiabilityCreateViewModel
        {

        }

        public class LiabilityViewModel
        {
            public int Id { get; set; }
            public string Source { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? Value { get; set; }
            public string InterestType { get; set; }

            [DisplayFormat(DataFormatString = "{0:P2}")]
            public double? InterestRate { get; set; }
            public string InterestRatePerX { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public DateTime? StartDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public DateTime? EndDate { get; set; }
            [DisplayFormat(DataFormatString = "{0:N0}")]
            public int PaymentPeriod { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double MonthlyInterestPayment { get; set; }

            public double OriginalInterestPayment { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double MonthlyOriginalPayment { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double TotalMonthlyPayment { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double TotalPayment { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double RemainedValue { get; set; }
            public string Status { get; set; }
            public string StatusCode { get; set; }
        }
    }
}