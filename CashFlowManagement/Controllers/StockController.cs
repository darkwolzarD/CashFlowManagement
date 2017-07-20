using CashFlowManagement.Models;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    [CheckSessionTimeOutAttribute]
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult Index()
        {
            StockListViewModel model = StockQueries.GetStockByUser(UserQueries.GetCurrentUsername());
            return View(model);
        }

        public ActionResult _Create()
        {
            StockCreateViewModel model = new StockCreateViewModel();
            HttpContext.Session["LIABILITIES"] = null;
            HttpContext.Session["STOCK"] = null;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Create(StockCreateViewModel model)
        {
            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            double totalLiabilityValue = 0;
            if (liabilities != null)
            {
                totalLiabilityValue = liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0);
            }

            if (model.StockValue < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if(StockQueries.CheckExistStock(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("DuplicateName", "Cổ phiếu này đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                HttpContext.Session["STOCK"] = model;
                return Content("success");
            }
            else
            {
                return PartialView("_StockForm", model);
            }
        }

        public ActionResult _StockForm()
        {
            StockCreateViewModel model = (StockCreateViewModel)HttpContext.Session["STOCK"];
            if (model == null)
            {
                model = new StockCreateViewModel();
            }
            return PartialView(model);
        }

        public ActionResult _StockUpdateForm(int id)
        {
            StockUpdateViewModel model = StockQueries.GetStockById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _StockUpdateForm(StockUpdateViewModel model)
        {
            double totalLiabilityValue = StockLiabilityQueries.GetLiabilityValueOfStock(model.Id);
            if (model.StockValue < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            StockUpdateViewModel stock = StockQueries.GetStockById(model.Id);

            if (!stock.Name.Equals(model.Name) && StockQueries.CheckExistStock(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("DuplicateName", "Cổ phiếu này đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                int result = StockQueries.UpdateStock(model);
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

        public ActionResult _StockTable()
        {
            StockListViewModel model = StockQueries.GetStockByUser(UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public ActionResult _LiabilityForm()
        {
            StockLiabilityCreateViewModel model = new StockLiabilityCreateViewModel();
            return PartialView(model);
        }

        public ActionResult _LiabilityForm2nd(int id)
        {
            StockLiabilityCreateViewModel model = new StockLiabilityCreateViewModel();
            model.AssetId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityForm2nd(StockLiabilityCreateViewModel model)
        {
            double totalLiabilityValue = StockLiabilityQueries.GetLiabilityValueOfStock(model.AssetId);
            double stockValue = StockQueries.GetStockValue(model.AssetId);
            if (stockValue < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
            }

            if (ModelState.IsValid)
            {
                int result = StockLiabilityQueries.AddStockLiability(model);
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
            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            StockLiabilityCreateViewModel model = liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        public ActionResult _LiabilityUpdateForm2nd(int id)
        {
            StockLiabilityUpdateViewModel model = StockLiabilityQueries.GetViewModelById(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _LiabilityUpdateForm2nd(StockLiabilityUpdateViewModel model)
        {
            double totalLiabilityValue = StockLiabilityQueries.GetLiabilityValueOfStock(model.AssetId);
            double liabilityValue = StockLiabilityQueries.GetStockValue(model.Id);
            double stockValue = StockQueries.GetStockValue(model.AssetId);
            if (stockValue < totalLiabilityValue - liabilityValue + model.Value && totalLiabilityValue - liabilityValue + model.Value > 0)
            {
                ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if (ModelState.IsValid)
            {
                int result = StockLiabilityQueries.UpdateStockLiability(model);
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
        public ActionResult _LiabilityUpdateForm(StockLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                StockLiabilityCreateViewModel updateModel = liabilities.Liabilities.Where(x => x.Id == model.Id).FirstOrDefault();

                updateModel.Value = model.Value;

                StockCreateViewModel stock = (StockCreateViewModel)HttpContext.Session["STOCK"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (stock.StockValue < totalLiabilityValue && totalLiabilityValue > 0)
                {
                    ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
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
            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            StockLiabilityListViewModel model = new StockLiabilityListViewModel();
            if (liabilities == null)
            {
                liabilities = new StockLiabilityListCreateViewModel();
            }
            foreach (var liability in liabilities.Liabilities)
            {
                StockLiabilityViewModel viewModel = new StockLiabilityViewModel();
                viewModel.Id = liability.Id;
                viewModel.Source = liability.Source;
                viewModel.Value = liability.Value;
                viewModel.InterestRate = liability.InterestRate / 100;
                viewModel.InterestRatePerX = StockLiabilityQueries.Helper.GetInterestTypePerX(liability.InterestRatePerX);
                viewModel.StartDate = liability.StartDate;
                viewModel.EndDate = liability.EndDate;
                viewModel.InterestType = StockLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                viewModel.PaymentPeriod = StockLiabilityQueries.Helper.CalculateTimePeriod(viewModel.StartDate.Value, viewModel.EndDate.Value);
                model.Liabilities.Add(viewModel);
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult _LiabilityForm(StockLiabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                StockCreateViewModel stock = (StockCreateViewModel)HttpContext.Session["STOCK"];
                double totalLiabilityValue = liabilities != null ? liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0) : 0;

                if (stock.StockValue < totalLiabilityValue + model.Value && totalLiabilityValue + model.Value > 0)
                {
                    ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị bất động sản");
                    return PartialView(model);
                }
                else
                {
                    if (liabilities == null)
                    {
                        liabilities = new StockLiabilityListCreateViewModel();
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

            StockCreateViewModel model = (StockCreateViewModel)HttpContext.Session["STOCK"];
            StockViewModel viewModel = new StockViewModel();
            viewModel.Name = model.Name;
            viewModel.Transactions.Transactions.Add(new StockTransactionViewModel
            {
                NumberOfStock = model.NumberOfStock,
                SpotRice =  model.SpotRice,
                StockValue = model.StockValue,
                ExpectedDividend = model.ExpectedDividend
            });

            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];

            if (liabilities != null && liabilities.Liabilities.Count > 0)
            {
                foreach (var liability in liabilities.Liabilities)
                {
                    StockLiabilityViewModel liabilityViewModel = new StockLiabilityViewModel();
                    liabilityViewModel.Source = liability.Source;
                    liabilityViewModel.Value = liability.Value;
                    liabilityViewModel.InterestType = StockLiabilityQueries.Helper.GetInterestType(liability.InterestType);
                    liabilityViewModel.InterestRatePerX = StockLiabilityQueries.Helper.GetInterestTypePerX(liability.InterestRatePerX);
                    liabilityViewModel.InterestRate = liability.InterestRate / 100;
                    liabilityViewModel.StartDate = liability.StartDate.Value;
                    liabilityViewModel.EndDate = liability.EndDate.Value;
                    liabilityViewModel.PaymentPeriod = StockLiabilityQueries.Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, liabilityViewModel.EndDate.Value);

                    if (liabilityViewModel.StartDate <= current && current <= liabilityViewModel.EndDate)
                    {
                        int currentPeriod = StockLiabilityQueries.Helper.CalculateTimePeriod(liabilityViewModel.StartDate.Value, DateTime.Now);
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

                    viewModel.Transactions.Transactions.FirstOrDefault().Liabilities.Liabilities.Add(liabilityViewModel);
                }

                viewModel.TotalLiabilityValue = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.Value.HasValue ? y.Value.Value : 0));
                viewModel.TotalOriginalPayment = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.MonthlyOriginalPayment));
                viewModel.TotalInterestPayment = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.MonthlyInterestPayment));
                viewModel.TotalMonthlyPayment = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.TotalMonthlyPayment));
                viewModel.TotalPayment = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.TotalPayment));
                viewModel.TotalRemainedValue = viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Sum(x => x.Sum(y => y.RemainedValue));
                viewModel.TotalInterestRate = viewModel.TotalInterestPayment / viewModel.TotalLiabilityValue * 12;
                viewModel.RowSpan = viewModel.Transactions.Transactions.Any() ? viewModel.Transactions.Transactions.Count() + viewModel.Transactions.Transactions.Select(x => x.Liabilities.Liabilities).Count() + 4 : 4;

                if (viewModel.Transactions.Transactions.Any())
                {
                    viewModel.RowSpan = 4;
                    bool flag = false;
                    foreach (var transaction in viewModel.Transactions.Transactions)
                    {
                        if (transaction.Liabilities.Liabilities.Count() > 0)
                        {
                            if (flag == false)
                            {
                                flag = true;
                            }
                            viewModel.RowSpan += transaction.Liabilities.Liabilities.Count();
                        }
                    }
                    if (flag == true)
                    {
                        viewModel.RowSpan += 1;
                    }
                }
                else
                {
                    viewModel.RowSpan = 4;
                }
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Save(StockCreateViewModel model)
        {
            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            double totalLiabilityValue = 0;
            if (liabilities != null)
            {
                totalLiabilityValue = liabilities.Liabilities.Sum(x => x.Value.HasValue ? x.Value.Value : 0);
            }

            if (model.StockValue < totalLiabilityValue && totalLiabilityValue > 0)
            {
                ModelState.AddModelError("CompareStockValueAndLiabilityValue", "Giá trị tổng số nợ không vượt quá giá trị góp vốn kinh doanh");
            }

            if (StockQueries.CheckExistStock(UserQueries.GetCurrentUsername(), model.Name))
            {
                ModelState.AddModelError("DuplicateName", "Cổ phiếu này đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                model.Liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
                string user = UserQueries.GetCurrentUsername();
                int result = StockQueries.CreateStock(model, user);
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
                return PartialView("_StockForm", model);
            }
        }

        [HttpPost]
        public ActionResult DeleteTempLiability(int id)
        {
            StockLiabilityListCreateViewModel liabilities = (StockLiabilityListCreateViewModel)HttpContext.Session["LIABILITIES"];
            liabilities.Liabilities.Remove(liabilities.Liabilities.Where(x => x.Id == id).FirstOrDefault());
            return Content("success");
        }


        [HttpPost]
        public ActionResult DeleteLiability(int id)
        {
            int result = StockLiabilityQueries.DeleteStockLiability(id);
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
        public ActionResult DeleteStock(int id)
        {
            int result = StockQueries.DeleteStock(id);
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