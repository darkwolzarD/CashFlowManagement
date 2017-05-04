﻿using CashFlowManagement.EntityModel;
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

        public PartialViewResult _SellAssetModal(int assetId)
        {
            AssetViewModel model = new AssetViewModel();
            model.Asset = new Assets();
            model.Asset.Id = assetId;
            return PartialView(model);
        }

        public JsonResult SellAsset(AssetViewModel model)
        {
            int result = AssetQueries.SellAsset(model.Asset.Id, model.SellAmount);
            return Json(new { result = result });
        }

        public JsonResult CheckAvailableMoney()
        {
            double result = AssetQueries.CheckAvailableMoney("test");
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}