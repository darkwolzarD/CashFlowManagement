using CashFlowManagement.EntityModel;
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
        // GET: Income
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult SalaryTable()
        {
            List<Salary> model = SalaryQueries.GetSalaryByUser("test");
            return PartialView(model);
        }

        public PartialViewResult SalaryModal()
        {
            Salary model = new Salary();
            return PartialView(model);
        }

        public PartialViewResult UpdateSalaryModal(int id)
        {
            Salary model = SalaryQueries.GetSalaryById(id);
            return PartialView(model);
        }

        public JsonResult CreateSalary(Salary model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            //code here//
            //processing//
            //-->model
            int result = SalaryQueries.CreateSalary(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateSalary(Salary model)
        {
            int result = SalaryQueries.UpdateSalary(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteSalary(int id)
        {
            int result = SalaryQueries.DeleteSalary(id);
            return Json(new { result = result });
        }
    }
}