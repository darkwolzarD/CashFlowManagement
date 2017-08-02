using CashFlowManagement.EntityModel;
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
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.Session["USER"] != null)
            {
                var user = (Users)HttpContext.Session["USER"];
                if (!user.IncomeInitialized)
                {
                    return RedirectToAction("Index", "Salary");
                }
                else if (!user.RealEstateInitialized)
                {
                    return RedirectToAction("Index", "RealEstate");
                }
                else if (!user.BusinessInitialized)
                {
                    return RedirectToAction("Index", "Business");
                }
                else if (!user.BankDepositInitialized)
                {
                    return RedirectToAction("Index", "BankDeposit");
                }
                else if (!user.StockInitialized)
                {
                    return RedirectToAction("Index", "Stock");
                }
                else if (!user.InsuranceInitialized)
                {
                    return RedirectToAction("Index", "Insurance");
                }
                else if (!user.OtherAssetInitialized)
                {
                    return RedirectToAction("Index", "OtherAsset");
                }
                else if (!user.CarLiabilityInitialized)
                {
                    return RedirectToAction("Index", "CarLiability");
                }
                else if (!user.CreditCardInitialized)
                {
                    return RedirectToAction("Index", "CreditCardLiability");
                }
                else if (!user.OtherLiabilityInitialized)
                {
                    return RedirectToAction("Index", "OtherLiability");
                }
                else if (!user.FamilyExpenseInitialized)
                {
                    return RedirectToAction("Index", "FamilyExpense");
                }
                else if (!user.OtherExpenseInitialized)
                {
                    return RedirectToAction("Index", "OtherExpense");
                }
                else if (!user.AvailableMoneyInitialized)
                {
                    return RedirectToAction("Index", "AvailableMoney");
                }
                else
                {
                    return RedirectToAction("Index", "FinancialStatus");
                }
            }
            else
            {
                return View();
            }
        }
        // GET: Account
        public ActionResult Login()
        {
            if (HttpContext.Session["USER"] != null)
            {
                var user = (Users)HttpContext.Session["USER"];
                if (!user.IncomeInitialized)
                {
                    return RedirectToAction("Index", "Salary");
                }
                else if (!user.RealEstateInitialized)
                {
                    return RedirectToAction("Index", "RealEstate");
                }
                else if (!user.BusinessInitialized)
                {
                    return RedirectToAction("Index", "Business");
                }
                else if (!user.BankDepositInitialized)
                {
                    return RedirectToAction("Index", "BankDeposit");
                }
                else if (!user.StockInitialized)
                {
                    return RedirectToAction("Index", "Stock");
                }
                else if (!user.InsuranceInitialized)
                {
                    return RedirectToAction("Index", "Insurance");
                }
                else if (!user.OtherAssetInitialized)
                {
                    return RedirectToAction("Index", "OtherAsset");
                }
                else if (!user.CarLiabilityInitialized)
                {
                    return RedirectToAction("Index", "CarLiability");
                }
                else if (!user.CreditCardInitialized)
                {
                    return RedirectToAction("Index", "CreditCardLiability");
                }
                else if (!user.OtherLiabilityInitialized)
                {
                    return RedirectToAction("Index", "OtherLiability");
                }
                else if (!user.FamilyExpenseInitialized)
                {
                    return RedirectToAction("Index", "FamilyExpense");
                }
                else if (!user.OtherExpenseInitialized)
                {
                    return RedirectToAction("Index", "OtherExpense");
                }
                else if (!user.AvailableMoneyInitialized)
                {
                    return RedirectToAction("Index", "AvailableMoney");
                }
                else
                {
                    return RedirectToAction("Index", "FinancialStatus");
                }
            }
            else
            {
                return View();
            }
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
                    return RedirectToAction("Index", "Salary");
                }
                else if (!user.RealEstateInitialized)
                {
                    return RedirectToAction("Index", "RealEstate");
                }
                else if (!user.BusinessInitialized)
                {
                    return RedirectToAction("Index", "Business");
                }
                else if (!user.BankDepositInitialized)
                {
                    return RedirectToAction("Index", "BankDeposit");
                }
                else if (!user.StockInitialized)
                {
                    return RedirectToAction("Index", "Stock");
                }
                else if (!user.InsuranceInitialized)
                {
                    return RedirectToAction("Index", "Insurance");
                }
                else if (!user.OtherAssetInitialized)
                {
                    return RedirectToAction("Index", "OtherAsset");
                }
                else if (!user.CarLiabilityInitialized)
                {
                    return RedirectToAction("Index", "CarLiability");
                }
                else if (!user.CreditCardInitialized)
                {
                    return RedirectToAction("Index", "CreditCardLiability");
                }
                else if (!user.OtherLiabilityInitialized)
                {
                    return RedirectToAction("Index", "OtherLiability");
                }
                else if (!user.FamilyExpenseInitialized)
                {
                    return RedirectToAction("Index", "FamilyExpense");
                }
                else if (!user.OtherExpenseInitialized)
                {
                    return RedirectToAction("Index", "OtherExpense");
                }
                else if (!user.AvailableMoneyInitialized)
                {
                    return RedirectToAction("Index", "AvailableMoney");
                }
                else
                {
                    return RedirectToAction("Index", "FinancialStatus");
                }
            }
            else
            {
                ModelState.AddModelError("LoginError", "Sai tên đăng nhập hoặc mật khẩu");
                return View(model);
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
            if (UserQueries.CheckUniqueUser(model.Email))
            {
                ModelState.AddModelError("CheckUniqueUser", "Email này đã được đăng ký, đề nghị quý khách liên hệ quản trị viên để lấy lại mật khẩu");
            }
            if (UserQueries.CheckUniquePhoneNumber(model.PhoneNumber))
            {
                ModelState.AddModelError("CheckUniquePhoneNumber", "Số điện thoại này đã được đăng ký, đề nghị quý khách liên hệ quản trị viên để lấy lại mật khẩu");
            }
            if (ModelState.IsValid)
            {
                Users user = UserQueries.Register(model);
                HttpContext.Session["USER"] = user;
                return Content("success");
            }
            else return PartialView(model);
        }

        [CheckSessionTimeOut]
        public ActionResult SalaryInitialize()
        {
            int result = UserQueries.SalaryInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "RealEstate");
        }

        [CheckSessionTimeOut]
        public ActionResult RealEstateInitialize()
        {
            int result = UserQueries.RealEstateInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Business");
        }

        [CheckSessionTimeOut]
        public ActionResult BusinessInitialize()
        {
            int result = UserQueries.BusinessInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "BankDeposit");
        }

        [CheckSessionTimeOut]
        public ActionResult BankDepositInitialize()
        {
            int result = UserQueries.BankDepositInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Stock");
        }

        [CheckSessionTimeOut]
        public ActionResult StockInitialize()
        {
            int result = UserQueries.StockInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Insurance");
        }

        [CheckSessionTimeOut]
        public ActionResult InsuranceInitialize()
        {
            int result = UserQueries.InsuranceInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherAsset");
        }

        [CheckSessionTimeOut]
        public ActionResult OtherAssetInitialize()
        {
            int result = UserQueries.OtherAssetInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "CarLiability");
        }

        [CheckSessionTimeOut]
        public ActionResult CarLiabilityInitialize()
        {
            int result = UserQueries.CarLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "CreditCardLiability");
        }

        [CheckSessionTimeOut]
        public ActionResult CreditCardLiabilityInitialize()
        {
            int result = UserQueries.CreditCardLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherLiability");
        }

        [CheckSessionTimeOut]
        public ActionResult OtherLiabilityInitialize()
        {
            int result = UserQueries.OtherLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "FamilyExpense");
        }

        [CheckSessionTimeOut]
        public ActionResult FamilyExpenseInitialize()
        {
            int result = UserQueries.FamilyExpenseInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherExpense");
        }

        [CheckSessionTimeOut]
        public ActionResult OtherExpenseInitialize()
        {
            int result = UserQueries.OtherExpenseInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "AvailableMoney");
        }

        [CheckSessionTimeOut]
        public ActionResult AvailableMoneyInitialize()
        {
            int result = UserQueries.AvailableMoneyInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "FinancialStatus");
        }
    }
}