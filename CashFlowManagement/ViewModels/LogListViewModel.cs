using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class LogListViewModel
    {
        public int Type { get; set; }
        public List<Log> List { get; set; }
        public List<Assets> Stocks { get; set; }

        public LogListViewModel()
        {
            List = new List<Log>();
            Stocks = new List<Assets>();
        }
    }
}