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
    public class AssetController : Controller
    {
        // GET: BandDeposit
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult AssetTable(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser("test", type);
            return PartialView(model);
        }

        //public PartialViewResult UpdateBankDepositModal(int id)
        //{
        //    BankDepositIncomes model = BankDepositQueries.GetBankDepositById(id);
        //    return PartialView(model);
        //}

        public JsonResult CreateAsset(AssetViewModel model)
        {
            int type = 0;
            if(model.SpecificAsset is BankDeposits)
            {
                type = (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT
            }
            model.Asset.Username = "test";
            int result = AssetQueries.CreateAsset(model, type);
            return Json(new { result = result });
        }

        //public JsonResult UpdateBankDeposit(BankDepositIncomes model)
        //{
        //    int result = BankDepositQueries.UpdateBankDeposit(model);
        //    return Json(new { result = result });
        //}

        //public JsonResult DeleteBankDeposit(int id)
        //{
        //    int result = BankDepositQueries.DeleteBankDeposit(id);
        //    return Json(new { result = result });
        //}
    }
}