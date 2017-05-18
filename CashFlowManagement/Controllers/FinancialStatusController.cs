using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels;
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
            FinancialStatusViewModel model = FinancialStatusQueries.GetFinancialStatusByUser(UserQueries.GetCurrentUsername());
            return View(model);
        }
    }
}