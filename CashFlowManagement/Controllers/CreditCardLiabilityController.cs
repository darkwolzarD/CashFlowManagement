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
    public class CreditCardLiabilityController : Controller
    {
        // GET: CreditCard
        public ActionResult Index()
        {
            bool result = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(result);
        }

        public ActionResult _CreditCardLiabilityForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _CreditCardLiabilityForm(CreditCardLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = CreditCardLiabilityQueries.AddCreditCardLiability(model, UserQueries.GetCurrentUsername());
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
                return PartialView(model);
            }
        }

        public ActionResult _CreditCardLiabilityUpdateForm(int id)
        {
            CreditCardLiabilityUpdateViewModel model = CreditCardLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _CreditCardLiabilityUpdateForm(CreditCardLiabilityUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = CreditCardLiabilityQueries.UpdateCreditCardLiability(model);
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
                return PartialView(model);
            }
        }

        public ActionResult _CreditCardLiabilityTable()
        {
            CreditCardLiabilityListViewModel model = CreditCardLiabilityQueries.GetCreditCardLiabilityByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteCreditCardLiability(int id)
        {
            int result = CreditCardLiabilityQueries.DeleteCreditCardLiability(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }
    }
}