using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CashFlowManagement.Queries.BankDepositQueries;
using static CashFlowManagement.Queries.OtherAssetLiabilityQueries;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class InsuranceController : Controller
    {
        // GET: Insurance
        public ActionResult Index()
        {
            bool result = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(result);
        }

        public ActionResult _InsuranceForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _InsuranceForm(InsuranceCreateViewModel model)
        {
            if (model.EndDate < DateTime.Now)
            {
                ModelState.AddModelError("CheckEndDate", "Hợp đồng bảo hiểm này đã hết hạn, vui lòng chỉ nhập hợp đồng bảo hiểm đang hiệu lực");
            }

            if (model.StartDate > DateTime.Now)
            {
                ModelState.AddModelError("CheckStartDate", "Ngày bắt đầu phải nhỏ hơn ngày hiện tại.");
            }

            if (model.Expense * CarLiabilityQueries.Helper.CalculateTimePeriod(model.StartDate.Value, model.EndDate.Value) >= model.Value)
            {
                ModelState.AddModelError("CheckValueAndTotalExpenseError", "Tổng số tiền đóng phải nhỏ hơn tiền thụ hưởng");
            }

            if (ModelState.IsValid)
            {
                int result = InsuranceQueries.CreateInsurance(model, UserQueries.GetCurrentUsername());
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
                return PartialView(model);
            }
        }

        public ActionResult _InsuranceUpdateForm(int id)
        {
            InsuranceUpdateViewModel model = InsuranceQueries.GetInsuranceById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _InsuranceUpdateForm(InsuranceUpdateViewModel model)
        {

            if (model.EndDate < DateTime.Now)
            {
                ModelState.AddModelError("CheckEndDate", "Hợp đồng bảo hiểm này đã hết hạn, vui lòng chỉ nhập hợp đồng bảo hiểm đang hiệu lực");
            }

            if (model.StartDate > DateTime.Now)
            {
                ModelState.AddModelError("CheckStartDate", "Ngày bắt đầu phải nhỏ hơn ngày hiện tại.");
            }

            if (model.Expense * CarLiabilityQueries.Helper.CalculateTimePeriod(model.StartDate.Value, model.EndDate.Value) >= model.Value)
            {
                ModelState.AddModelError("CheckValueAndTotalExpenseError", "Tổng số tiền đóng phải nhỏ hơn tiền thụ hưởng");
            }

            if (ModelState.IsValid)
            {
                int result = InsuranceQueries.UpdateInsurance(model);
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
                return PartialView(model);
            }
        }

        public ActionResult _InsuranceTable()
        {
            InsuranceListViewModel model = InsuranceQueries.GetInsuranceByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteInsurance(int id)
        {
            int result = InsuranceQueries.DeleteInsurance(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }

        public ActionResult _OtherAssetForm()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _OtherAssetForm(OtherAssetCreateViewModel model)
        {
            if (OtherAssetQueries.CheckExistOtherAsset(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistAsset", "Tài sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                string user = UserQueries.GetCurrentUsername();
                int result = OtherAssetQueries.CreateOtherAsset(model, user);
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
                model.IsInDebt = false;
                return PartialView("_OtherAssetForm", model);
            }
        }

        public ActionResult _OtherAssetUpdateForm(int id)
        {
            OtherAssetUpdateViewModel model = OtherAssetQueries.GetOtherAssetById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _OtherAssetUpdateForm(OtherAssetUpdateViewModel model)
        {
            var asset = OtherAssetQueries.GetOtherAssetById(model.Id);
            if (!asset.Name.Equals(model.Name) && OtherAssetQueries.CheckExistOtherAsset(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistAsset", "Tài sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                int result = OtherAssetQueries.UpdateOtherAsset(model);
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
                return PartialView(model);
            }
        }

        public ActionResult _OtherAssetTable()
        {
            OtherAssetListViewModel model = OtherAssetQueries.GetOtherAssetByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult DeleteOtherAsset(int id)
        {
            int result = OtherAssetQueries.DeleteOtherAsset(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }

        public ActionResult _AssetSummary()
        {
            AssetSummaryViewModel model = new AssetSummaryViewModel();
            string username = UserQueries.GetCurrentUsername();
            model.RealEstates = RealEstateQueries.GetRealEstateSummaryByUser(username);
            model.Businesses = BusinessQueries.GetBusinessSummaryByUser(username);
            model.BankDeposits = BankDepositQueries.GetBankDepositSummaryByUser(username);
            model.Stocks = StockQueries.GetStockSummaryByUser(username);
            model.Insurances = InsuranceQueries.GetInsuranceSummaryByUser(username);
            model.OtherAssets = OtherAssetQueries.GetOtherAssetSummaryByUser(username);
            return PartialView(model);
        }
    }
}