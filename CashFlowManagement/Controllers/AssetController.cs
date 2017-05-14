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
        public ActionResult AssetTable(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser("test", type);
            return View(model);
        }

        public ActionResult Report(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser("test", type);
            return View(model);
        }

        public PartialViewResult _AssetUpdateModal(int assetId, int transactionId)
        {
            AssetViewModel model = AssetQueries.GetAssetById(assetId, transactionId);
            if(model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
               && model.Transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY)
            {
                model.CurrentAvailableMoney = AssetQueries.CheckAvailableMoney(model.Asset.Username, model.Transaction.TransactionDate);
            }
            return PartialView(model);
        }

        public JsonResult CreateAsset(AssetViewModel model)
        {
            int type = model.Asset.AssetType;

            int result = AssetQueries.CreateAsset(model, type, "test");
            return Json(new { result = result });
        }

        public JsonResult BuyAsset(AssetViewModel model)
        {
            int result = AssetQueries.BuyAsset(model, "test");
            return Json(new { result = result });
        }

        public JsonResult UpdateAsset(AssetViewModel model)
        {
            int type = model.Asset.AssetType;

            int result = AssetQueries.UpdateAsset(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteAsset(int assetId)
        {
            int result = AssetQueries.DeleteAsset(assetId);
            return Json(new { result = result });
        }

        public PartialViewResult _SellAssetModal(int assetId, int assetType)
        {
            AssetViewModel model = new AssetViewModel();
            model.Asset = new Assets();
            model.Asset.AssetType = assetType;
            if (assetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE)
            {
                model.Asset.Id = assetId;
            }
            return PartialView(model);
        }

        public JsonResult SellAsset(AssetViewModel model)
        {
            int result = AssetQueries.SellAsset(model);
            return Json(new { result = result });
        }

        public JsonResult CheckAvailableMoney(DateTime? date)
        {
            int result = (int)AssetQueries.CheckAvailableMoney("test", date == null ? DateTime.Now : date.Value);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckRemainedStock(string stock)
        {
            double result = AssetQueries.CheckRemainedStock(stock);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}