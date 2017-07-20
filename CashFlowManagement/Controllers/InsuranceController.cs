﻿using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CashFlowManagement.Queries.BankDepositQueries;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class InsuranceController : Controller
    {
        // GET: Insurance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _InsuranceForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _InsuranceForm(InsuranceCreateViewModel model)
        {
            if(model.Expense * Helper.CalculateTimePeriod(model.StartDate.Value, model.EndDate.Value) >= model.Value)
            {
                ModelState.AddModelError("CheckValueAndTotalExpenseError", "Tổng số tiền đóng phải nhỏ hơn tiền thụ hưởng");
            }

            if (ModelState.IsValid)
            {
                int result = InsuranceQueries.CreateInsurance(model, UserQueries.GetCurrentUsername());
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
                return PartialView(model);
            }
        }

        public ActionResult _InsuranceUpdateForm(int id)
        {
            InsuranceUpdateViewModel model = InsuranceQueries.GetInsuranceById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _InsuranceUpdateForm(InsuranceUpdateViewModel model)
        {
            if (model.Expense * Helper.CalculateTimePeriod(model.StartDate.Value, model.EndDate.Value) >= model.Value)
            {
                ModelState.AddModelError("CheckValueAndTotalExpenseError", "Tổng số tiền đóng phải nhỏ hơn tiền thụ hưởng");
            }

            if (ModelState.IsValid)
            {
                int result = InsuranceQueries.UpdateInsurance(model);
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
                return PartialView(model);
            }
        }

        public ActionResult _InsuranceTable()
        {
            InsuranceListViewModel model = InsuranceQueries.GetInsuranceByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteInsurance(int id)
        {
            int result = InsuranceQueries.DeleteInsurance(id);
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