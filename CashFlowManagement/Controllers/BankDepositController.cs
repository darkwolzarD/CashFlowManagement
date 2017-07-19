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
    public class BankDepositController : Controller
    {
        // GET: BankDeposit
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _BankDepositForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _BankDepositForm(BankDepositCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = BankDepositQueries.CreateBankDeposit(model, UserQueries.GetCurrentUsername());
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

        public ActionResult _BankDepositUpdateForm(int id)
        {
            BankDepositUpdateViewModel model = BankDepositQueries.GetBankDepositById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _BankDepositUpdateForm(BankDepositUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = BankDepositQueries.UpdateBankDeposit(model);
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

        public ActionResult _BankDepositTable()
        {
            BankDepositListViewModel model = BankDepositQueries.GetBankDepositByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteBankDeposit(int id)
        {
            int result = BankDepositQueries.DeleteBankDeposit(id);
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