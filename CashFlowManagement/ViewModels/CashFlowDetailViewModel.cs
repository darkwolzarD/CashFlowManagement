using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class CashFlowDetailListViewModel
    {
        public double BeforeAvailableMoney { get; set; }
        public double AfterAvailableMoney { get; set; }
        public string Action { get; set; }
        public List<CashFlowDetailViewModel> CashflowDetails { get; set; }

        public CashFlowDetailListViewModel()
        {
            CashflowDetails = new List<CashFlowDetailViewModel>();
        }
    }

    public class CashFlowDetailViewModel
    {
        public string Month { get; set; }
        public double IncomeBefore { get; set; }
        public double IncomeAfter { get; set; }
    }
}