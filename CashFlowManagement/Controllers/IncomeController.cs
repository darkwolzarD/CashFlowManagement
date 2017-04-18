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
    public class IncomeController : Controller
    {
        // GET: Income
        public ActionResult IncomeTable(int type)
        {
            IncomeListViewModel model = IncomeQueries.GetIncomeByUser("test", type);
            return View(model);
        }

        //public PartialViewResult UpdateBankDepositModal(int id)
        //{
        //    BankDepositIncomes model = BankDepositQueries.GetBankDepositById(id);
        //    return PartialView(model);
        //}

        public PartialViewResult _IncomeUpdateModal(int incomeId)
        {
            Incomes model = IncomeQueries.GetIncomeById(incomeId);
            return PartialView(model);
        }

        public JsonResult CreateIncome(Incomes model)
        {
            int type = model.IncomeType;
            int result = IncomeQueries.CreateIncome(model, type, "test");
            return Json(new { result = result });
        }

        public JsonResult UpdateIncome(Incomes model)
        {
            int type = model.IncomeType;
            int result = IncomeQueries.UpdateIncome(model, "test");
            return Json(new { result = result });
        }

        public JsonResult DeleteIncome(int incomeId)
        {
            int result = IncomeQueries.DeleteIncome(incomeId);
            return Json(new { result = result });
        }
    }
}