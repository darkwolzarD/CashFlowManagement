﻿using CashFlowManagement.EntityModel;
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
                return RedirectToAction("Index", "Salary");
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
            if (ModelState.IsValid)
            {
                Users user = UserQueries.Register(model);
                HttpContext.Session["USER"] = user;
                return RedirectToAction("Index", "Salary");
            }
            else return PartialView(model);
        }

        public ActionResult SalaryInitialize()
        {
            int result = UserQueries.SalaryInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "RealEstate");
        }

        public ActionResult RealEstateInitialize()
        {
            int result = UserQueries.RealEstateInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Business");
        }

        public ActionResult BusinessInitialize()
        {
            int result = UserQueries.BusinessInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "BankDeposit");
        }

        public ActionResult BankDepositInitialize()
        {
            int result = UserQueries.BankDepositInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Stock");
        }

        public ActionResult StockInitialize()
        {
            int result = UserQueries.StockInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "Insurance");
        }

        public ActionResult InsuranceInitialize()
        {
            int result = UserQueries.InsuranceInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherAsset");
        }

        public ActionResult OtherAssetInitialize()
        {
            int result = UserQueries.OtherAssetInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "CarLiability");
        }

        public ActionResult CarLiabilityInitialize()
        {
            int result = UserQueries.CarLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "CreditCardLiability");
        }

        public ActionResult CreditCardLiabilityInitialize()
        {
            int result = UserQueries.CreditCardLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherLiability");
        }

        public ActionResult OtherLiabilityInitialize()
        {
            int result = UserQueries.OtherLiabilityInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "FamilyExpense");
        }

        public ActionResult FamilyExpenseInitialize()
        {
            int result = UserQueries.FamilyExpenseInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "OtherExpense");
        }

        public ActionResult OtherExpenseInitialize()
        {
            int result = UserQueries.OtherExpenseInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "AvailableMoney");
        }

        public ActionResult AvailableMoneyInitialize()
        {
            int result = UserQueries.AvailableMoneyInitialize(UserQueries.GetCurrentUsername());
            return RedirectToAction("Index", "FinancialStatus");
        }
    }
}