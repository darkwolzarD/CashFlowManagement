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

        public ActionResult _Create()
        {
            OtherAssetCreateViewModel model = new OtherAssetCreateViewModel();
            HttpContext.Session["LIABILITIES"] = null;
            HttpContext.Session["OTHER_ASSET"] = null;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Create(OtherAssetCreateViewModel model)
        {
            OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            double totalLiabilityValue = 0;
            if (liabilities != null)
            {
                totalLiabilityValue = liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0);
            }

            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị tài sản");
            }

            if (OtherAssetQueries.CheckExistOtherAsset(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistAsset", "Tài sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                HttpContext.Session["OTHER_ASSET"] = model;
                return Content("success");
            }
            else
            {
                return PartialView("_OtherAssetForm", model);
            }
        }

        public ActionResult _OtherAssetForm()
        {
            OtherAssetCreateViewModel model = (OtherAssetCreateViewModel)HttpContext.Session["OTHER_ASSET"];
            if (model == null)
            {
                model = new OtherAssetCreateViewModel();
            }
            return PartialView(model);
        }

        public ActionResult _OtherAssetUpdateForm(int id)
        {
            OtherAssetUpdateViewModel model = OtherAssetQueries.GetOtherAssetById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _OtherAssetUpdateForm(OtherAssetUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfOtherAsset(model.Id);
            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị tài sản");
            }

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

        public ActionResult _LiabilityForm()
        {
            OtherAssetLiabilityCreateViewModel model = new OtherAssetLiabilityCreateViewModel();
            return PartialView(model);
        }

        public ActionResult _LiabilityForm2nd(int id)
        {
            OtherAssetLiabilityCreateViewModel model = new OtherAssetLiabilityCreateViewModel();
            model.AssetId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityForm2nd(OtherAssetLiabilityCreateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfOtherAsset(model.AssetId);
            double otherAssetValue = OtherAssetQueries.GetOtherAssetValue(model.AssetId);
            if (otherAssetValue < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }

            if (ModelState.IsValid)
            {
                int result = OtherAssetLiabilityQueries.AddOtherAssetLiability(model);
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

        public ActionResult _LiabilityUpdateForm(int id)
        {
            OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            OtherAssetLiabilityCreateViewModel model = liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        public ActionResult _LiabilityUpdateForm2nd(int id)
        {
            OtherAssetLiabilityUpdateViewModel model = OtherAssetLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityUpdateForm2nd(OtherAssetLiabilityUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfOtherAsset(model.AssetId);
            double liabilityValue = GetLiabilityValue(model.Id);
            double otherAssetValue = OtherAssetQueries.GetOtherAssetValue(model.AssetId);
            if (otherAssetValue < totalLiabilityValue - liabilityValue + model.Value && totalLiabilityValue - liabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị tài sản");
            }

            if (ModelState.IsValid)
            {
                int result = OtherAssetLiabilityQueries.UpdateOtherAssetLiability(model);
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

        [HttpPost]
        public ActionResult _LiabilityUpdateForm(OtherAssetLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                OtherAssetLiabilityCreateViewModel updateModel = liabilities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();

                updateModel.Value = model.Value;

                OtherAssetCreateViewModel otherAsset = (OtherAssetCreateViewModel)HttpContext.Session["OTHER_ASSET"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (otherAsset.Value < totalLiabilityValue && totalLiabilityValue > 0)
                {
                    ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị tài sản");
                    return PartialView(model);
                }
                else
                {
                    updateModel.Id = model.Id;
                    updateModel.Source = model.Source;
                    updateModel.InterestType = model.InterestType;
                    updateModel.InterestRate = model.InterestRate;
                    updateModel.StartDate = model.StartDate;
                    updateModel.EndDate = model.EndDate;
                    HttpContext.Session["LIABILITIES"] = liabilities;
                    return Content("success");
                }
            }
            else
            {
                return PartialView(model);
            }
        }

        public ActionResult _LiabilityTable()
        {
            OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            OtherAssetLiabilityListViewModel model = new OtherAssetLiabilityListViewModel();
            if (liabilities == null)
            {
                liabilities = new OtherAssetLiabilityListCreateViewModel();
            }
            foreach (var liability in liabilities.Liabilities)
            {
                OtherAssetLiabilityViewModel viewModel = new OtherAssetLiabilityViewModel();
                viewModel.Id = liability.Id;
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = OtherAssetLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                viewModel.PaymentPeriod = Helper.CalculateTimePeriod(viewModel.StartDate.Value, viewModel.EndDate.Value);
                model.Liabilities.Add(viewModel);
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult _LiabilityForm(OtherAssetLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                OtherAssetCreateViewModel otherAsset = (OtherAssetCreateViewModel)HttpContext.Session["OTHER_ASSET"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (otherAsset.Value < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
                {
                    ModelState.AddModelError("CompareOtherAssetValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
                    return PartialView(model);
                }
                else
                {
                    if (liabilities == null)
                    {
                        liabilities = new OtherAssetLiabilityListCreateViewModel();
                    }
                    else
                    {
                        if (liabilities.Liabilities.Count > 0)
                        {
                            id = liabilities.Liabilities.Max(x => x.Id) + 1;
                        }
                        else
                        {
                            id = 1;
                        }
                    }
                    model.Id = id;
                    liabilities.Liabilities.Add(model);
                    HttpContext.Session["LIABILITIES"] = liabilities;
                    return Content("success");
                }
            }
            else
            {
                return PartialView(model);
            }
        }

        public ActionResult _Confirm()
        {
            DateTime current = DateTime.Now;

            OtherAssetCreateViewModel model = (OtherAssetCreateViewModel)HttpContext.Session["OTHER_ASSET"];
            OtherAssetViewModel viewModel = new OtherAssetViewModel();
            viewModel.Name = model.Name;
            viewModel.Value = model.Value.Value;
            viewModel.Income = model.Income.HasValue ? model.Income.Value : 0;
            viewModel.AnnualIncome = viewModel.Income * 12;
            viewModel.RentYield = viewModel.Value > 0 ? viewModel.Income / viewModel.Value : 0;

            OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            viewModel.RowSpan = liabilities != null && liabilities.Liabilities.Count > 0 ? liabilities.Liabilities.Count() + 3 : 2;

            if (liabilities != null && liabilities.Liabilities.Count > 0)
            {
                foreach (var liability in liabilities.Liabilities)
                {
                    OtherAssetLiabilityViewModel liabilityViewModel = new OtherAssetLiabilityViewModel();
                    liabilityViewModel.Source = liability.Source;
                    liabilityViewModel.Value = liability.Value;
                    liabilityViewModel.InterestType = Helper.GetInterestType(liability.InterestType);
                    liabilityViewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
                    liabilityViewModel.InterestRate = liability.InterestRate / 100;
                    liabilityViewModel.StartDate = liability.StartDate.Value;
                    liabilityViewModel.EndDate = liability.EndDate.Value;
                    liabilityViewModel.PaymentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, liabilityViewModel.EndDate.Value);

                    if (liabilityViewModel.StartDate <= current && current <= liabilityViewModel.EndDate)
                    {
                        int currentPeriod = Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, DateTime.Now);
                        double interestRate = liability.InterestRatePerX == (int)Constants.Constants.INTEREST_RATE_PER.MONTH ? liability.InterestRate.Value / 100 : liability.InterestRate.Value / 1200;
                        liabilityViewModel.OriginalInterestPayment = liabilityViewModel.Value.Value * interestRate;
                        //Fixed interest type
                        if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                        {
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.PaymentPeriod > 0 ? liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod : 0;
                            liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.Value.Value * interestRate;
                            liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                            liabilityViewModel.TotalPayment = liabilityViewModel.TotalMonthlyPayment * currentPeriod;
                            liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                            liabilityViewModel.Status = "Đang nợ";
                            liabilityViewModel.StatusCode = "label-success";
                        }
                        //Reduced interest type
                        else
                        {
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.PaymentPeriod > 0 ? liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod : 0;
                            liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * (currentPeriod + 1);
                            liabilityViewModel.MonthlyInterestPayment = (liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * currentPeriod) * interestRate;
                            liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                            liabilityViewModel.TotalPayment = interestRate * (currentPeriod * liabilityViewModel.Value.Value + (currentPeriod * (currentPeriod + 1) / 2) * liabilityViewModel.MonthlyOriginalPayment);
                            liabilityViewModel.Status = "Đang nợ";
                            liabilityViewModel.StatusCode = "label-success";
                        }
                    }
                    else
                    {
                        liabilityViewModel.MonthlyOriginalPayment = 0;
                        liabilityViewModel.MonthlyInterestPayment = 0;
                        liabilityViewModel.TotalMonthlyPayment = 0;
                        liabilityViewModel.TotalPayment = 0;
                        liabilityViewModel.RemainedValue = 0;
                    }

                    viewModel.Liabilities.Add(liabilityViewModel);
                }
            }

            var liabilitites = viewModel.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
            viewModel.TotalLiabilityValue = liabilitites.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
            viewModel.TotalOriginalPayment = liabilitites.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
            viewModel.TotalInterestPayment = liabilitites.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
            viewModel.TotalMonthlyPayment = liabilitites.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
            viewModel.TotalPayment = liabilitites.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
            viewModel.TotalRemainedValue = liabilitites.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
            viewModel.TotalInterestRate = viewModel.TotalLiabilityValue > 0 ? liabilitites.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / viewModel.TotalLiabilityValue * 12 : 0;

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Save(OtherAssetCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
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

        [HttpPost]
        public ActionResult DeleteTempLiability(int id)
        {
            OtherAssetLiabilityListCreateViewModel liabilities = (OtherAssetLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            liabilities.Liabilities.Remove(liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault());
            return Content("success");
        }


        [HttpPost]
        public ActionResult DeleteLiability(int id)
        {
            int result = OtherAssetLiabilityQueries.DeleteOtherAssetLiability(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }


        [HttpPost]
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