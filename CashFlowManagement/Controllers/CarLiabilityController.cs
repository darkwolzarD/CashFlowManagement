using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class CarLiabilityController : Controller
    {
        // GET: CarLiability
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _CarLiabilityForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _CarLiabilityForm(CarLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = CarLiabilityQueries.AddCarLiability(model, UserQueries.GetCurrentUsername());
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

        public ActionResult _CarLiabilityUpdateForm(int id)
        {
            CarLiabilityUpdateViewModel model = CarLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _CarLiabilityUpdateForm(CarLiabilityUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = CarLiabilityQueries.UpdateCarLiability(model);
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

        public ActionResult _CarLiabilityTable()
        {
            CarLiabilityListViewModel model = CarLiabilityQueries.GetCarLiabilityByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteCarLiability(int id)
        {
            int result = CarLiabilityQueries.DeleteCarLiability(id);
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