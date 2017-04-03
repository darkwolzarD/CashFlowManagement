using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class BankDepositController : Controller
    {
        // GET: BandDeposit
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult BankDepositTable()
        {
            List<BankDepositIncomes> model = BankDepositQueries.GetBankDepositByUser("test");
            return PartialView(model);
        }

        public PartialViewResult BankDepositModal()
        {
            BankDepositIncomes model = new BankDepositIncomes();
            return PartialView(model);
        }

        public PartialViewResult UpdateBankDepositModal(int id)
        {
            BankDepositIncomes model = BankDepositQueries.GetBankDepositById(id);
            return PartialView(model);
        }

        public JsonResult CreateBankDeposit(BankDepositIncomes model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = BankDepositQueries.CreateBankDeposit(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateBankDeposit(BankDepositIncomes model)
        {
            int result = BankDepositQueries.UpdateBankDeposit(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteBankDeposit(int id)
        {
            int result = BankDepositQueries.DeleteBankDeposit(id);
            return Json(new { result = result });
        }
    }
}