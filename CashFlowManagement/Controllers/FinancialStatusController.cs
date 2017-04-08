using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using CashFlowManagement.ViewModels.FinancialStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class FinancialStatusController : Controller
    {
        // GET: FinancialStatus
        public ActionResult Index()
        {
            FinancialStatusViewModel model = FinancialStatusProcessing.GetFinancialStatusByUser("test");
            return View(model);
        }
    }
}