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
    public class FamilyExpenseController : Controller
    {
        // GET: FamilyExpense
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _FamilyExpenseForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _FamilyExpenseForm(FamilyExpenseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = FamilyExpenseQueries.CreateFamilyExpense(model, UserQueries.GetCurrentUsername());
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

        public ActionResult _FamilyExpenseUpdateForm(int id)
        {
            FamilyExpenseUpdateViewModel model = FamilyExpenseQueries.GetFamilyExpenseById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _FamilyExpenseUpdateForm(FamilyExpenseUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = FamilyExpenseQueries.UpdateFamilyExpense(model);
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

        public ActionResult _FamilyExpenseTable()
        {
            FamilyExpenseListViewModel model = FamilyExpenseQueries.GetFamilyExpenseByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteFamilyExpense(int id)
        {
            int result = FamilyExpenseQueries.DeleteFamilyExpense(id);
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