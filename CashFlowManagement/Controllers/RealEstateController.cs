using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CashFlowManagement.Queries.RealEstateLiabilityQueries;

namespace CashFlowManagement.Controllers
{
    public class RealEstateController : Controller
    {
        // GET: RealEstate
        public ActionResult Index()
        {
            RealEstateListViewModel model = RealEstateQueries.GetRealEstateByUser(UserQueries.GetCurrentUsername());
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
            if (ModelState.IsValid)
            {
                HttpContext.Session["REAL_ESTATE"] = model;
                return Content("success");
            }
            else
            {
                return PartialView("_RealEstateForm",  model);
            }
        }

        public ActionResult _RealEstateForm()
        {
            RealEstateCreateViewModel model = (RealEstateCreateViewModel)HttpContext.Session["REAL_ESTATE"];
            if(model == null)
            {
                model = new RealEstateCreateViewModel();
            }
            return PartialView(model);
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
        public ActionResult _LiabilityUpdateForm(int id)
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            RealEstateLiabilityCreateViewModel model = liabilities.Liabilities.Where(x => x.Temp_Id == id).FirstOrDefault();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityUpdateForm(RealEstateLiabilityCreateViewModel model)
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            RealEstateLiabilityCreateViewModel updateModel = liabilities.Liabilities.Where(x => x.Temp_Id == model.Temp_Id).FirstOrDefault();
            updateModel.Temp_Id = model.Temp_Id;
            updateModel.Value = model.Value;
            updateModel.Source = model.Source;
            updateModel.InterestType = model.InterestType;
            updateModel.InterestRate = model.InterestRate;
            updateModel.StartDate = model.StartDate;
            updateModel.EndDate = model.EndDate;
            HttpContext.Session["LIABILITIES"] = liabilities;
            return PartialView(model);
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
                viewModel.Temp_Id = liability.Temp_Id;
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.InterestRatePerX = Helper.GetInterestTypePerX(liability.InterestRatePerX);
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = RealEstateLiabilityQueries.Helper.GetInterestType(liability.InterestType);
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
                if (liabilities == null)
                {
                    liabilities = new RealEstateLiabilityListCreateViewModel();
                }
                else
                {
                    id = liabilities.Liabilities.Max(x => x.Temp_Id) + 1;
                }
                model.Temp_Id = id;
                liabilities.Liabilities.Add(model);
                HttpContext.Session["LIABILITIES"] = liabilities;
                return Content("success");
            }
            else
            {
                return View(model);
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
            viewModel.RentYield = viewModel.Income / viewModel.Value;

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
                        //Fixed interest type
                        if (liability.InterestType == (int)Constants.Constants.INTEREST_TYPE.FIXED)
                        {
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                            liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.Value.Value * liabilityViewModel.InterestRate.Value / 12;
                            liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                            liabilityViewModel.TotalPayment = liabilityViewModel.TotalMonthlyPayment * currentPeriod;
                            liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.TotalPayment;
                        }
                        //Reduced interest type
                        else
                        {
                            liabilityViewModel.MonthlyOriginalPayment = liabilityViewModel.Value.Value / liabilityViewModel.PaymentPeriod;
                            liabilityViewModel.RemainedValue = liabilityViewModel.Value.Value - liabilityViewModel.MonthlyOriginalPayment * currentPeriod;
                            liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.RemainedValue * liabilityViewModel.InterestRate.Value / 12;
                            liabilityViewModel.TotalMonthlyPayment = liabilityViewModel.MonthlyOriginalPayment + liabilityViewModel.MonthlyInterestPayment;
                            liabilityViewModel.TotalPayment = liabilityViewModel.InterestRate.Value / 12 * (currentPeriod * liabilityViewModel.Value.Value + currentPeriod * (currentPeriod + 1) / 2 * liabilityViewModel.MonthlyOriginalPayment);
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

                viewModel.TotalLiabilityValue = viewModel.Liabilities.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                viewModel.TotalOriginalPayment = viewModel.Liabilities.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestPayment = viewModel.Liabilities.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalMonthlyPayment = viewModel.Liabilities.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalPayment = viewModel.Liabilities.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                viewModel.TotalRemainedValue = viewModel.Liabilities.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                viewModel.TotalInterestRate = viewModel.TotalInterestPayment / viewModel.TotalLiabilityValue * 12;
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Save(RealEstateCreateViewModel model)
        {
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
                model.IsInDept = false;
                return PartialView("_RealEstateForm", model);
            }
        }

        [HttpPost]
        public ActionResult DeleteLiability(int id)
        {
            RealEstateLiabilityListCreateViewModel liabilities = (RealEstateLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            liabilities.Liabilities.Remove(liabilities.Liabilities.Where(x => x.Temp_Id == id).FirstOrDefault());
            return Content("success");
        }
    }
}