using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CashFlowManagement.Queries.BusinessLiabilityQueries;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class BusinessController : Controller
    {
        // GET: Business
        public ActionResult Index()
        {
            bool model = UserQueries.IsCompleteInitialized(UserQueries.GetCurrentUsername());
            return View(model);
        }

        public ActionResult _Create()
        {
            BusinessCreateViewModel model = new BusinessCreateViewModel();
            HttpContext.Session["LIABILITIES"] = null;
            HttpContext.Session["BUSINESS"] = null;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Create(BusinessCreateViewModel model)
        {
            BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            double totalLiabilityValue = 0;
            if (liabilities != null)
            {
                totalLiabilityValue = liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0);
            }

            if (BusinessQueries.CheckExistBusiness(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistBusiness", "Kinh doanh này đã tồn tại, vui lòng nhập tên khác");
            }

            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if (ModelState.IsValid)
            {
                HttpContext.Session["BUSINESS"] = model;
                return Content("success");
            }
            else
            {
                return PartialView("_BusinessForm", model);
            }
        }

        public ActionResult _BusinessForm()
        {
            BusinessCreateViewModel model = (BusinessCreateViewModel)HttpContext.Session["BUSINESS"];
            if (model == null)
            {
                model = new BusinessCreateViewModel();
            }
            return PartialView(model);
        }

        public ActionResult _BusinessUpdateForm(int id)
        {
            BusinessUpdateViewModel model = BusinessQueries.GetBusinessById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _BusinessUpdateForm(BusinessUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfBusiness(model.Id);
            if (model.Value < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            var business = BusinessQueries.GetBusinessById(model.Id);
            if (!business.Name.Equals(model.Name) && BusinessQueries.CheckExistBusiness(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistBusiness", "Kinh doanh này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                int result = BusinessQueries.UpdateBusiness(model);
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

        public ActionResult _BusinessTable()
        {
            BusinessListViewModel model = BusinessQueries.GetBusinessByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult _LiabilityForm()
        {
            BusinessLiabilityCreateViewModel model = new BusinessLiabilityCreateViewModel();
            return PartialView(model);
        }

        public ActionResult _LiabilityForm2nd(int id)
        {
            BusinessLiabilityCreateViewModel model = new BusinessLiabilityCreateViewModel();
            model.AssetId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityForm2nd(BusinessLiabilityCreateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfBusiness(model.AssetId);
            double businessValue = BusinessQueries.GetBusinessValue(model.AssetId);
            if (businessValue < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if (ModelState.IsValid)
            {
                int result = BusinessLiabilityQueries.AddBusinessLiability(model);
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
            BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            BusinessLiabilityCreateViewModel model = liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        public ActionResult _LiabilityUpdateForm2nd(int id)
        {
            BusinessLiabilityUpdateViewModel model = BusinessLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityUpdateForm2nd(BusinessLiabilityUpdateViewModel model)
        {
            double totalLiabilityValue = GetLiabilityValueOfBusiness(model.AssetId);
            double liabilityValue = GetLiabilityValue(model.Id);
            double businessValue = BusinessQueries.GetBusinessValue(model.AssetId);
            if (businessValue < totalLiabilityValue - liabilityValue + model.Value && totalLiabilityValue - liabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if (ModelState.IsValid)
            {
                int result = BusinessLiabilityQueries.UpdateBusinessLiability(model);
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
        public ActionResult _LiabilityUpdateForm(BusinessLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                BusinessLiabilityCreateViewModel updateModel = liabilities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();

                updateModel.Value = model.Value;

                BusinessCreateViewModel business = (BusinessCreateViewModel)HttpContext.Session["BUSINESS"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (business.Value < totalLiabilityValue && totalLiabilityValue > 0)
                {
                    ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
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
            BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            BusinessLiabilityListViewModel model = new BusinessLiabilityListViewModel();
            if (liabilities == null)
            {
                liabilities = new BusinessLiabilityListCreateViewModel();
            }
            foreach (var liability in liabilities.Liabilities)
            {
                BusinessLiabilityViewModel viewModel = new BusinessLiabilityViewModel();
                viewModel.Id = liability.Id;
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = BusinessLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                viewModel.PaymentPeriod = Helper.CalculateTimePeriod(viewModel.StartDate.Value, viewModel.EndDate.Value);
                model.Liabilities.Add(viewModel);
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult _LiabilityForm(BusinessLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                BusinessCreateViewModel business = (BusinessCreateViewModel)HttpContext.Session["BUSINESS"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (business.Value < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
                {
                    ModelState.AddModelError("CompareBusinessValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
                    return PartialView(model);
                }
                else
                {
                    if (liabilities == null)
                    {
                        liabilities = new BusinessLiabilityListCreateViewModel();
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

            BusinessCreateViewModel model = (BusinessCreateViewModel)HttpContext.Session["BUSINESS"];
            BusinessViewModel viewModel = new BusinessViewModel();
            viewModel.Name = model.Name;
            viewModel.Value = model.Value.Value;
            viewModel.Income = model.Income.HasValue ? model.Income.Value : 0;
            viewModel.AnnualIncome = viewModel.Income * 12;
            viewModel.RentYield = viewModel.AnnualIncome / viewModel.Value;

            BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            viewModel.RowSpan = liabilities != null && liabilities.Liabilities.Count > 0 ? liabilities.Liabilities.Count() + 3 : 2;

            if (liabilities != null && liabilities.Liabilities.Count > 0)
            {
                foreach (var liability in liabilities.Liabilities)
                {
                    BusinessLiabilityViewModel liabilityViewModel = new BusinessLiabilityViewModel();
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
                            liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability);
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
                            liabilityViewModel.TotalPayment = RealEstateLiabilityQueries.Helper.CalculateAnnualPayment(liability);
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
                viewModel.TotalLiabilityValue =liabilitites.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                viewModel.TotalOriginalPayment =liabilitites.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestPayment =liabilitites.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalMonthlyPayment =liabilitites.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalPayment =liabilitites.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalRemainedValue =liabilitites.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestRate = viewModel.TotalLiabilityValue > 0 ? liabilitites.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / viewModel.TotalLiabilityValue * 12 : 0;
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Save(BusinessCreateViewModel model)
        {
            if (BusinessQueries.CheckExistBusiness(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("CheckExistBusiness", "Kinh doanh này đã tồn tại, vui lòng nhập tên khác");
            }

            if (ModelState.IsValid)
            {
                model.Liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                string user = UserQueries.GetCurrentUsername();
                int result = BusinessQueries.CreateBusiness(model, user);
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
                return PartialView("_BusinessForm", model);
            }
        }

        [HttpPost]
        public ActionResult DeleteTempLiability(int id)
        {
            BusinessLiabilityListCreateViewModel liabilities = (BusinessLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            liabilities.Liabilities.Remove(liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault());
            return Content("success");
        }


        [HttpPost]
        public ActionResult DeleteLiability(int id)
        {
            int result = BusinessLiabilityQueries.DeleteBusinessLiability(id);
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
        public ActionResult DeleteBusiness(int id)
        {
            int result = BusinessQueries.DeleteBusiness(id);
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
