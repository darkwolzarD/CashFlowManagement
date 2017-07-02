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
            RealEstateCreateViewModel model = new RealEstateCreateViewModel();
            HttpContext.Session["LIABILITIES"] = null;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RealEstateCreateViewModel model)
        {
            model.Liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            int result = RealEstateQueries.CreateRealEstate(model, UserQueries.GetCurrentUsername());
            if(result > 0)
            {
                return Content("Tạo bất động sản thành công");
            }
            else
            {
                return Content("Có lỗi xảy ra");
            }
        }

        public ActionResult _LiabilityForm()
        {
            RealEstateLiabilityCreateViewModel model = new RealEstateLiabilityCreateViewModel();
            return View(model);
        }
        public ActionResult _LiabilityTable()
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            RealEstateLiabilityListViewModel model = new RealEstateLiabilityListViewModel();
            if (liabilities == null)
            {
                liabilities = new RealEstateLiabilityListCreateViewModel();
            }
            foreach (var liability in liabilities.Liabilities)
            {
                RealEstateLiabilityViewModel viewModel = new RealEstateLiabilityViewModel();
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = RealEstateLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                model.Liabilities.Add(viewModel);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult _LiabilityForm(RealEstateLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                if (liabilities == null)
                {
                    liabilities = new RealEstateLiabilityListCreateViewModel();
                }
                liabilities.Liabilities.Add(model);
                HttpContext.Session["LIABILITIES"] = liabilities;
                return Content("success");
            }
            else
            {
                return View(model);
            }
        }
    }
}