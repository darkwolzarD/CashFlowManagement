﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Models
{
    public class StockCreateViewModel
    {
        [Required(ErrorMessage = "Nhập mã cổ phiếu")]
        [Display(Name = "Mã cổ phiếu")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nhập số lượng cổ phiếu")]
        [Display(Name = "Số lượng cổ phiếu")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? NumberOfStock { get; set; }

        [Required(ErrorMessage = "Nhập giá hiện tại")]
        [Display(Name = "Giá hiện tại")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? SpotRice { get; set; }

        [Required(ErrorMessage = "Nhập tổng giá trị hiện tại")]
        [Display(Name = "Tổng giá trị hiện tại")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? StockValue { get; set; }

        [Required(ErrorMessage = "Nhập cổ tức mong đợi năm tới")]
        [Display(Name = "Cổ tức mong đợi năm tới")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(1, Double.MaxValue, ErrorMessage = "Cổ tức phải lớn hơn 1%")]
        public double? ExpectedDividend { get; set; }
        [Display(Name = "Ghi chú")]
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

        public string Note { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalLiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        public double TotalOriginalInterestPayment { get; set; }

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
        public StockTransactionListViewModel Transactions { get; set; }

        public StockViewModel()
        {
            Transactions = new StockTransactionListViewModel();
        }
    }

    public class StockListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        public List<StockViewModel> Stocks { get; set; }
        public bool IsInitialized { get; set; }

        public StockListViewModel()
        {
            Stocks = new List<StockViewModel>();
        }
    }

    public class StockSummaryViewModel
    {
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int NumberOfStock { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double SpotRice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double StockValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double ExpectedDividend { get; set; }

        public string Note { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double LiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double InterestRate { get; set; }

        public double OriginalInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AnnualPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double RemainedValue { get; set; }
    }

    public class StockSummaryListViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalNumberOfStock { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalStockValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalLiabilityValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double TotalInterestRate { get; set; }

        public string TotalInterestRatePerX { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyInterestPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalMonthlyPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalAnnualPayment { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalRemainedValue { get; set; }

        public List<StockSummaryViewModel> StockSummaries { get; set; }

        public StockSummaryListViewModel()
        {
            StockSummaries = new List<StockSummaryViewModel>();
        }
    }
}