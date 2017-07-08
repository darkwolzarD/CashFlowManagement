using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class StockViewModels
    {
        public class StockCreateViewModel
        {
            [Required(ErrorMessage = "Nhập tên cổ phiếu")]
            [Display(Name = "Tên cổ phiếu")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Nhập số lượng cổ phiếu")]
            [Display(Name = "Số lượng cổ phiếu")]
            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? NumberOfStock { get; set; }

            [Required(ErrorMessage = "Nhập giá cổ phiếu")]
            [Display(Name = "Giá cổ phiếu")]
            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? SpotRice { get; set; }

            [Required(ErrorMessage = "Nhập tổng giá trị cổ phiếu")]
            [Display(Name = "Tổng giá trị")]
            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? StockValue { get; set; }

            [Required(ErrorMessage = "Nhập cổ tức mong đợi")]
            [Display(Name = "Cổ tức mong đợi")]
            [DisplayFormat(DataFormatString = "{0:N2}")]
            [Range(1, Double.MaxValue, ErrorMessage = "Cổ tức phải lớn hơn 1%")]
            public int? ExpectedDividend { get; set; }
            public string Note { get; set; }

            public StockLiabilityListCreateViewModel Liabilities { get; set; }

            public bool IsInDebt { get; set; }

            public StockCreateViewModel()
            {
                Liabilities = new StockLiabilityListCreateViewModel();
            }
        }

        public class StockUpdateViewModel : StockCreateViewModel
        {
            public int Id { get; set; }
        }

        public class StockViewModel
        {
            public int Id { get; set; }
            
            public string Name { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? NumberOfStock { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? SpotRice { get; set; }

            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double? StockValue { get; set; }

            [DisplayFormat(DataFormatString = "{0:N2}")]
            public int? ExpectedDividend { get; set; }

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

            public int RowSpan { get; set; }
            public List<StockLiabilityViewModel> Liabilities { get; set; }

            public StockViewModel()
            {
                Liabilities = new List<StockLiabilityViewModel>();
            }
        }

        public class StockListViewModel
        {
            [DisplayFormat(DataFormatString = "{0:N0}")]
            public double TotalValue { get; set; }
            
            public List<StockViewModel> Stocks { get; set; }

            public StockListViewModel()
            {
                Stocks = new List<StockViewModel>();
            }
        }
    }
}