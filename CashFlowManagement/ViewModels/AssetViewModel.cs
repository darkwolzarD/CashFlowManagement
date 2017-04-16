using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.ViewModels
{
    public class AssetViewModel
    {
        public Assets Asset { get; set; }
        public Incomes Income { get; set; }
    }
}