using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
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
        public ActionResult Index()
        {
            List<Log> model = LogQueries.GetLogByUser("test", string.Empty);
            return View(model);
        }

        public JsonResult CreateLog(Log model)
        {
            int result = LogQueries.CreateLog(model, "test");
            return Json(new { result = result });
        }
    }
}