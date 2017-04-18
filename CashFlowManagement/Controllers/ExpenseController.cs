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
    public class ExpenseController : Controller
    {
        // GET: Expense
        public ActionResult ExpenseTable(int type)
        {
            ExpenseListViewModel model = ExpenseQueries.GetExpenseByUser("test", type);
            return View(model);
        }

        public PartialViewResult _ExpenseUpdateModal(int expenseId)
        {
            Expenses model = ExpenseQueries.GetExpenseById(expenseId);
            return PartialView(model);
        }

        public JsonResult CreateExpense(Expenses model)
        {
            int type = model.ExpenseType;

            int result = ExpenseQueries.CreateExpense(model, type, "test");
            return Json(new { result = result });
        }

        public JsonResult UpdateExpense(Expenses model)
        {
            int type = model.ExpenseType;

            int result = ExpenseQueries.UpdateExpense(model, "test");
            return Json(new { result = result });
        }

        public JsonResult DeleteExpense(int expenseId)
        {
            int result = ExpenseQueries.DeleteExpense(expenseId);
            return Json(new { result = result });
        }
    }
}