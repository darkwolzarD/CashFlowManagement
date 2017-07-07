using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class SalaryController : Controller
    {
        // GET: Salary
        public ActionResult Index()
        {
            return View();
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
                return PartialView();
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
                return PartialView();
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
    }
}