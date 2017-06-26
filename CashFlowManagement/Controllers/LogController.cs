using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index(int type)
        {

            LogListViewModel model = LogQueries.GetLogByUser(UserQueries.GetCurrentUsername(), type);
            return View(model);
        }

        public JsonResult CreateLog(Log model)
        {
            int result = LogQueries.CreateLog(model, UserQueries.GetCurrentUsername());
            return Json(new { result = result });
        }
    }
}