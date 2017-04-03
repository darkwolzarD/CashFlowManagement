using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
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
            FinancialStatus model = FinancialStatusQueries.GetCurrentFinancialStatus();
            return View(model);
        }
    }
}