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
                if (!user.IncomeInitialized)
                {
                    return RedirectToAction("Initialize", "Income");
                }
                else if (!user.RealEstateInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE });
                }
                else if (!user.BusinessInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.BUSINESS });
                }
                else if (!user.BankDepositInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT });
                }
                else if (!user.StockInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.STOCK });
                }
                else if (!user.InsuranceInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.INSURANCE });
                }
                else if (!user.OtherAssetInitialized)
                {
                    return RedirectToAction("Initialize", "Asset", new { type = (int)Constants.Constants.ASSET_TYPE.OTHERS });
                }
                else if (!user.CarLiabilityInitialized)
                {
                    return RedirectToAction("Initialize", "Liability", new { type = (int)Constants.Constants.LIABILITY_TYPE.CAR });
                }
                else if (!user.CreditCardInitialized)
                {
                    return RedirectToAction("Initialize", "Liability", new { type = (int)Constants.Constants.LIABILITY_TYPE.CREDIT_CARD });
                }
                else if (!user.OtherLiabilityInitialized)
                {
                    return RedirectToAction("Initialize", "Liability", new { type = (int)Constants.Constants.LIABILITY_TYPE.OTHERS });
                }
                else if (!user.FamilyExpenseInitialized)
                {
                    return RedirectToAction("Initialize", "Expense", new { type = (int)Constants.Constants.EXPENSE_TYPE.FAMILY });
                }
                else if (!user.OtherExpenseInitialized)
                {
                    return RedirectToAction("Initialize", "Expense", new { type = (int)Constants.Constants.EXPENSE_TYPE.OTHERS });
                }
                else {
                    return RedirectToAction("Index", "FinancialStatus");
                }
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
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = UserQueries.Register(model);
                HttpContext.Session["USER"] = user;
                return RedirectToAction("Initialize", "Income");
            }
            else return View(model);
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