using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashFlowManagement.Queries;

namespace CashFlowManagement.Controllers
{
    public class BusinessController : Controller
    {
        // GET: Business
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult BusinessTable()
        {
            List<BusinessIncomes> model = BusinessQueries.GetBusinessByUser("test");
            return PartialView(model);
        }

        public PartialViewResult BusinessModal()
        {
            BusinessIncomes model = new BusinessIncomes();
            return PartialView(model);
        }

        public PartialViewResult UpdateBusinessModal(int id)
        {
            BusinessIncomes model = BusinessQueries.GetBusinessById(id);
            return PartialView(model);
        }

        public JsonResult CreateBusiness(BusinessIncomes model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = BusinessQueries.CreateBusiness(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateBusiness(BusinessIncomes model)
        {
            int result = BusinessQueries.UpdateBusiness(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteBusiness(int id)
        {
            int result = BusinessQueries.DeleteBusiness(id);
            return Json(new { result = result });
        }
    }
}