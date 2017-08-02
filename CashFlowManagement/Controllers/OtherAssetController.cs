using CashFlowManagement.Models;
using CashFlowManagement.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using static CashFlowManagement.Queries.OtherAssetLiabilityQueries;

namespace CashFlowManagement.Queries
{
    [CheckSessionTimeOutAttribute]
    public class OtherAssetController : Controller
    {
        // GET: OtherAsset
        public ActionResult Index()
        {
            bool result = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(result);
        }
    }
}