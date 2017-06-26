using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
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
        public ActionResult Initialize(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser(UserQueries.GetCurrentUsername(), type);
            return View(model);
        }
        public ActionResult _InitializeConfirmation()
        {
            return PartialView();
        }
        public ActionResult AssetTable(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser(UserQueries.GetCurrentUsername(), type);
            return View(model);
        }

        public ActionResult _Report(int type)
        {
            AssetListViewModel model = AssetQueries.GetAssetByUser(UserQueries.GetCurrentUsername(), type);
            return View(model);
        }

        public PartialViewResult _AssetUpdateModal(int assetId, int transactionId)
        {
            AssetViewModel model = AssetQueries.GetAssetById(assetId, transactionId);
            if(model.Asset.AssetType == (int)Constants.Constants.ASSET_TYPE.STOCK
               && model.Transaction.TransactionType == (int)Constants.Constants.TRANSACTION_TYPE.BUY)
            {
                model.Transaction.Assets1.Value = 0 - model.Transaction.Assets1.Value;
                model.CurrentAvailableMoney = AssetQueries.CheckAvailableMoney(model.Asset.Username, model.Transaction.TransactionDate) + model.Asset.Value;
            }
            return PartialView(model);
        }

        public JsonResult CreateAsset(AssetViewModel model)
        {
            int type = model.Asset.AssetType;

            int result = AssetQueries.CreateAsset(model, type, UserQueries.GetCurrentUsername());
            return Json(new { result = result });
        }

        public JsonResult BuyAsset(AssetViewModel model)
        {
            int result = AssetQueries.BuyAsset(model, UserQueries.GetCurrentUsername());
            return Json(new { result = result });
        }

        public JsonResult UpdateAsset(AssetViewModel model)
        {
            int type = model.Asset.AssetType;

            int result = AssetQueries.UpdateAsset(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteAsset(int assetId, int transactionId)
        {
            int result = AssetQueries.DeleteAsset(assetId, transactionId);
            return Json(new { result = result });
        }

        public PartialViewResult _SellAssetModal(int assetId, int assetType)
        {
            AssetViewModel model = new AssetViewModel();
            model.Asset = new Assets();
            model.Asset.AssetType = assetType;
            if (assetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE || assetType == (int)Constants.Constants.ASSET_TYPE.BANK_DEPOSIT)
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

        public ActionResult CheckAvailableMoney(DateTime? date)
        {
            double result = (double)AssetQueries.CheckAvailableMoney(UserQueries.GetCurrentUsername(), date == null ? DateTime.Now : date.Value);
            return Content(FormatUtility.DisplayThousandSeparatorsForNumber(result));
        }

        public JsonResult CheckRemainedStock(string stock)
        {
            double result = AssetQueries.CheckRemainedStock(stock);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}