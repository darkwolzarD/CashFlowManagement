using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class AvailableMoneyController : Controller
    {
        // GET: AvailableMoney
        public ActionResult Index()
        {
            bool result = AvailableMoneyQueries.CheckExistAvailableMoney(UserQueries.GetCurrentUsername());
            return View(result);
        }

        public ActionResult CheckExistAvailableMoney()
        {
            bool result = AvailableMoneyQueries.CheckExistAvailableMoney(UserQueries.GetCurrentUsername());
            return Content(result.ToString());
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

        public ActionResult _AvailableMoneySummary()
        {
            AvailableMoneySummaryViewModel model = AvailableMoneyQueries.GetInitializedAvailableMoneySummary(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }
    }
}