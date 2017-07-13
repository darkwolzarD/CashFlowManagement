using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class AvailableMoneyController : Controller
    {
        // GET: AvailableMoney
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _AvailableMoneyTable()
        {
            AvailableMoneyViewModel model = AvailableMoneyQueries.GetInitializedAvailableMoney(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult _AvailableMoneyForm()
        {
            AvailableMoneyCreateViewModel model = AvailableMoneyQueries.GetInitializedAvailableMoney(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _AvailableMoneyForm(AvailableMoneyCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = AvailableMoneyQueries.CreateAvailableMoney(model, UserQueries.GetCurrentUsername());
                if(result > 0)
                {
                    return Content("success");
                }
                else
                {
                    return Content("failed");
                }
            }
            else
            {
                return PartialView(model);
            }
        }
    }
}