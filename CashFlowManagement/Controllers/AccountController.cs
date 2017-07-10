using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            Users user = UserQueries.CheckLogin(model);
            if (user != null)
            {
                HttpContext.Session["USER"] = user;
                return RedirectToAction("Index", "Navigation");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session["USER"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = UserQueries.Register(model);
                HttpContext.Session["USER"] = user;
                return RedirectToAction("Index", "Navigation");
            }
            else return PartialView(model);
        }

        public ActionResult IncomeInitialize()
        {
            Users user = (Users)HttpContext.Session["USER"];
            user = UserQueries.IncomeInitialize(user.Username);
            HttpContext.Session["USER"] = user;
            return RedirectToAction("Initialize", "Asset", new { @type = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE });
        }

        public ActionResult OtherAssetInitialize()
        {
            Users user = (Users)HttpContext.Session["USER"];
            user = UserQueries.OtherAssetInitialize(user.Username);
            HttpContext.Session["USER"] = user;
            return RedirectToAction("Initialize", "Liability", new { @type = (int)Constants.Constants.LIABILITY_TYPE.CAR });
        }

        public ActionResult OtherLiabilityInitialize()
        {
            Users user = (Users)HttpContext.Session["USER"];
            user = UserQueries.OtherLiabilityInitialize(user.Username);
            HttpContext.Session["USER"] = user;
            return RedirectToAction("Initialize", "Expense", new { @type = (int)Constants.Constants.EXPENSE_TYPE.FAMILY });
        }

        public ActionResult OtherExpenseInitialize()
        {
            Users user = (Users)HttpContext.Session["USER"];
            user = UserQueries.OtherExpenseInitialize(user.Username);
            HttpContext.Session["USER"] = user;
            return RedirectToAction("Index", "FinancialStatus");
        }
    }
}