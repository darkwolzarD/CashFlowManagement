using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class RealEstateController : Controller
    {
        // GET: RealEstate
        public ActionResult Index()
        {
            RealEstateListViewModel model = RealEstateQueries.GetRealEstateByUser(UserQueries.GetCurrentUsername());
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}