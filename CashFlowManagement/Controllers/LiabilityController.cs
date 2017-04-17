using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class LiabilityController : Controller
    {
        // GET: Liability
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult _LiabilityModal(int assetId, int type)
        {
            Liabilities model = new Liabilities
            {
                AssetId = assetId,
                LiabilityType = type == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE ? (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE : 0
            };
            return PartialView(model);
        }

        public PartialViewResult _LiabilityUpdateModal(int id, string trigger)
        {
            Liabilities liability = LiabilityQueries.GetLiabilityById(id);
            if(trigger.Equals("edit-no-rate"))
            {
                liability.InterestRate = 0;
            }
            return PartialView(liability);
        }

        public PartialViewResult _PaymentPerMonth(int id)
        {
            Liabilities liability = LiabilityQueries.GetLiabilityById(id);
            List<Liabilities> liabilityList = LiabilityQueries.GetLiabilityListById(id);
            List<LiabilityPaymentViewModel> paymentList = LiabilityQueries.CalculatePaymentsByMonth(liabilityList, liability, false);
            return PartialView(paymentList);
        }

        public JsonResult CreateLiability(Liabilities model)
        {
            int result = LiabilityQueries.CreateLiability(model, "test");
            return Json(new { result = result });
        }
    }
}