using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CashFlowManagement.Queries.RealEstateLiabilityQueries;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class RealEstateController : Controller
    {
        // GET: RealEstate
        public ActionResult Index()
        {
            bool model = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(model);
        }

        public ActionResult _Create()
        {
            RealEstateCreateViewModel model = new RealEstateCreateViewModel();
            HttpContext.Session["LIABILITIES"] = null;
            HttpContext.Session["REAL_ESTATE"] = null;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Create(RealEstateCreateViewModel model)
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            double totalLiabilityValue = 0;
            if (liabilities != null)
            {
                totalLiabilityValue = liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0);
            }

            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }

            if (RealEstateQueries.CheckExistRealEstate(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistRealEstate", "Bất động sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                HttpContext.Session["REAL_ESTATE"] = model;
                return Content("success");
            }
            else
            {
                return PartialView("_RealEstateForm", model);
            }
        }

        public ActionResult _RealEstateForm()
        {
            RealEstateCreateViewModel model = (RealEstateCreateViewModel)HttpContext.Session["REAL_ESTATE"];
            if (model == null)
            {
                model = new RealEstateCreateViewModel();
            }
            return PartialView(model);
        }

        public ActionResult _RealEstateUpdateForm(int id)
        {
            RealEstateUpdateViewModel model = RealEstateQueries.GetRealEstateById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _RealEstateUpdateForm(RealEstateUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfRealEstate(model.Id);
            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }
            var realEstate = RealEstateQueries.GetRealEstateById(model.Id);
            if (!realEstate.Name.Equals(model.Name) && RealEstateQueries.CheckExistRealEstate(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistRealEstate", "Bất động sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                int result = RealEstateQueries.UpdateRealEstate(model);
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

        public ActionResult _RealEstateTable()
        {
            RealEstateListViewModel model = RealEstateQueries.GetRealEstateByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult _LiabilityForm()
        {
            RealEstateLiabilityCreateViewModel model = new RealEstateLiabilityCreateViewModel();
            return PartialView(model);
        }

        public ActionResult _LiabilityForm2nd(int id)
        {
            RealEstateLiabilityCreateViewModel model = new RealEstateLiabilityCreateViewModel();
            model.AssetId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityForm2nd(RealEstateLiabilityCreateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfRealEstate(model.AssetId);
            double realEstateValue = RealEstateQueries.GetRealEstateValue(model.AssetId);
            if (realEstateValue < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }

            if (ModelState.IsValid)
            {
                int result = RealEstateLiabilityQueries.AddRealEstateLiability(model);
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
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            RealEstateLiabilityCreateViewModel model = liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        public ActionResult _LiabilityUpdateForm2nd(int id)
        {
            RealEstateLiabilityUpdateViewModel model = RealEstateLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityUpdateForm2nd(RealEstateLiabilityUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfRealEstate(model.AssetId);
            double liabilityValue = GetLiabilityValue(model.Id);
            double realEstateValue = RealEstateQueries.GetRealEstateValue(model.AssetId);
            if (realEstateValue < totalLiabilityValue - liabilityValue + model.Value && totalLiabilityValue - liabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }

            if (ModelState.IsValid)
            {
                int result = RealEstateLiabilityQueries.UpdateRealEstateLiability(model);
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
        public ActionResult _LiabilityUpdateForm(RealEstateLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                RealEstateLiabilityCreateViewModel updateModel = liabilities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();

                updateModel.Value = model.Value;

                RealEstateCreateViewModel realEstate = (RealEstateCreateViewModel)HttpContext.Session["REAL_ESTATE"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (realEstate.Value < totalLiabilityValue && totalLiabilityValue > 0)
                {
                    ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
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
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            RealEstateLiabilityListViewModel model = new RealEstateLiabilityListViewModel();
            if (liabilities == null)
            {
                liabilities = new RealEstateLiabilityListCreateViewModel();
            }
            foreach (var liability in liabilities.Liabilities)
            {
                RealEstateLiabilityViewModel viewModel = new RealEstateLiabilityViewModel();
                viewModel.Id = liability.Id;
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = RealEstateLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                viewModel.PaymentPeriod = Helper.CalculateTimePeriod(viewModel.StartDate.Value, viewModel.EndDate.Value);
                model.Liabilities.Add(viewModel);
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult _LiabilityForm(RealEstateLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                RealEstateCreateViewModel realEstate = (RealEstateCreateViewModel)HttpContext.Session["REAL_ESTATE"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (realEstate.Value < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
                {
                    ModelState.AddModelError("CompareRealEstateValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
                    return PartialView(model);
                }
                else
                {
                    if (liabilities == null)
                    {
                        liabilities = new RealEstateLiabilityListCreateViewModel();
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

            RealEstateCreateViewModel model = (RealEstateCreateViewModel)HttpContext.Session["REAL_ESTATE"];
            RealEstateViewModel viewModel = new RealEstateViewModel();
            viewModel.Name = model.Name;
            viewModel.Value = model.Value.Value;
            viewModel.Income = model.Income.HasValue ? model.Income.Value : 0;
            viewModel.AnnualIncome = viewModel.Income * 12;
            viewModel.RentYield = viewModel.AnnualIncome / viewModel.Value;

            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            viewModel.RowSpan = liabilities != null && liabilities.Liabilities.Count > 0 ? liabilities.Liabilities.Count() + 3 : 2;

            if (liabilities != null && liabilities.Liabilities.Count > 0)
            {
                foreach (var liability in liabilities.Liabilities)
                {
                    RealEstateLiabilityViewModel liabilityViewModel = new RealEstateLiabilityViewModel();
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
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
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
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
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

                var liabilitites = viewModel.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
                viewModel.TotalLiabilityValue = liabilitites.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                viewModel.TotalOriginalPayment = liabilitites.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestPayment = liabilitites.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalMonthlyPayment = liabilitites.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalPayment = liabilitites.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalRemainedValue = liabilitites.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestRate = viewModel.TotalLiabilityValue > 0 ? liabilitites.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / viewModel.TotalLiabilityValue * 12 : 0;
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Save(RealEstateCreateViewModel model)
        {
            if (RealEstateQueries.CheckExistRealEstate(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistRealEstate", "Bất động sản này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                model.Liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                string user = UserQueries.GetCurrentUsername();
                int result = RealEstateQueries.CreateRealEstate(model, user);
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
                return PartialView("_RealEstateForm", model);
            }
        }

        [HttpPost]
        public ActionResult DeleteTempLiability(int id)
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            liabilities.Liabilities.Remove(liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault());
            return Content("success");
        }


        [HttpPost]
        public ActionResult DeleteLiability(int id)
        {
            int result = RealEstateLiabilityQueries.DeleteRealEstateLiability(id);
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
        public ActionResult DeleteRealEstate(int id)
        {
            int result = RealEstateQueries.DeleteRealEstate(id);
            if (result > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("failed");
            }
        }
    }
}