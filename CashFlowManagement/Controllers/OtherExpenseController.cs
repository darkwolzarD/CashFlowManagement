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
    public class OtherExpenseController : Controller
    {
        // GET: OtherExpense
        public ActionResult Index()
        {
            bool result = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(result);
        }

        public ActionResult _OtherExpenseForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _OtherExpenseForm(OtherExpenseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = OtherExpenseQueries.CreateOtherExpense(model, UserQueries.GetCurrentUsername());
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

        public ActionResult _OtherExpenseUpdateForm(int id)
        {
            OtherExpenseUpdateViewModel model = OtherExpenseQueries.GetOtherExpenseById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _OtherExpenseUpdateForm(OtherExpenseUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = OtherExpenseQueries.UpdateOtherExpense(model);
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

        public ActionResult _OtherExpenseTable()
        {
            OtherExpenseListViewModel model = OtherExpenseQueries.GetOtherExpenseByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteOtherExpense(int id)
        {
            int result = OtherExpenseQueries.DeleteOtherExpense(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }

        public ActionResult _ExpenseSummary()
        {
            ExpenseSummaryViewModel model = new ExpenseSummaryViewModel();
            string username = UserQueries.GetCurrentUsername();
            model.FamilyExpenses = FamilyExpenseQueries.GetFamilyExpenseSummaryByUser(username);
            model.OtherExpenses = OtherExpenseQueries.GetOtherExpenseSummaryByUser(username);
            return PartialView(model);
        }
    }
}