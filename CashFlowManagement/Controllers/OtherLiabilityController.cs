using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class OtherLiabilityController : Controller
    {
        // GET: OtherLiability
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _OtherLiabilityForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _OtherLiabilityForm(OtherLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = OtherLiabilityQueries.AddOtherLiability(model, UserQueries.GetCurrentUsername());
                if (result > 0)
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
                return PartialView();
            }
        }

        public ActionResult _OtherLiabilityUpdateForm(int id)
        {
            OtherLiabilityUpdateViewModel model = OtherLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _OtherLiabilityUpdateForm(OtherLiabilityUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = OtherLiabilityQueries.UpdateOtherLiability(model);
                if (result > 0)
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
                return PartialView();
            }
        }

        public ActionResult _OtherLiabilityTable()
        {
            OtherLiabilityListViewModel model = OtherLiabilityQueries.GetOtherLiabilityByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteOtherLiability(int id)
        {
            int result = OtherLiabilityQueries.DeleteOtherLiability(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }

        public ActionResult _LiabilitySummary()
        {
            LiabilitySummaryViewModel model = new LiabilitySummaryViewModel();
            string username = UserQueries.GetCurrentUsername();
            model.CarLiabilities = CarLiabilityQueries.GetCarLiabilitySummaryByUser(username);
            model.CreditCardLiabilities = CreditCardLiabilityQueries.GetCreditCardLiabilitySummaryByUser(username);
            model.OtherLiabilities = OtherLiabilityQueries.GetOtherLiabilitySummaryByUser(username);
            return PartialView(model);
        }
    }
}