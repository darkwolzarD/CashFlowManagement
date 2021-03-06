﻿using CashFlowManagement.Models;
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
    public class SalaryController : Controller
    {
        // GET: Salary
        public ActionResult Index()
        {
            bool result = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(result);
        }

        public ActionResult _SalaryForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _SalaryForm(SalaryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = SalaryQueries.CreateSalary(model, UserQueries.GetCurrentUsername());
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

        public ActionResult _SalaryUpdateForm(int id)
        {
            SalaryUpdateViewModel model = SalaryQueries.GetSalaryById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _SalaryUpdateForm(SalaryUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = SalaryQueries.UpdateSalary(model);
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

        public ActionResult _SalaryTable()
        {
            SalaryListViewModel model = SalaryQueries.GetSalaryByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteSalary(int id)
        {
            int result = SalaryQueries.DeleteSalary(id);
            if(result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }

        public ActionResult _SalarySummary()
        {
            SalarySummaryListViewModel model = SalaryQueries.GetSalarySummaryByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult Finish()
        {
            return RedirectToAction("Index", "RealEstate");
        }
    }
}