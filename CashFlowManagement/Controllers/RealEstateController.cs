using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels.RealEstate;
using CashFlowManagement.Queries;
using CashFlowManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class RealEstateController : Controller
    {
        // GET: RealEstate
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult RealEstateTable()
        {
            List<RealEstateIncomes> ListRealEstateIncomes = RealEstateQueries.GetRealEstateByUser("test");
            RealEstateListViewModel result = RealEstateProcessing.GetRealEstateListViewModel(ListRealEstateIncomes);
            return PartialView(result);
        }

        public PartialViewResult RealEstateModal()
        {
            RealEstateIncomes model = new RealEstateIncomes();
            return PartialView(model);
        }

        public PartialViewResult UpdateRealEstateModal(int id)
        {
            RealEstateIncomes model = RealEstateQueries.GetRealEstateById(id);
            return PartialView(model);
        }

        public PartialViewResult LoanModal(int id)
        {
            Loans model = new Loans();
            model.RealEstateIncomeId = id;
            return PartialView(model);
        }

        public PartialViewResult UpdateLoanModal(int id)
        {
            Loans model = RealEstateQueries.GetLoanById(id);
            return PartialView(model);
        }

        public PartialViewResult UpdateLoanWithRateModal(int id)
        {
            Loans model = RealEstateQueries.GetLoanById(id);
            return PartialView(model);
        }

        public JsonResult CreateRealEstate(RealEstateIncomes model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = RealEstateQueries.CreateRealEstate(model);
            return Json(new { result = result });
        }

        public JsonResult CreateLoan(Loans model)
        {
            DateTime current = DateTime.Now;
            model.CreatedDate = new DateTime(current.Year, current.Month, 1);
            int result = RealEstateQueries.CreateLoan(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateRealEstate(RealEstateIncomes model)
        {
            int result = RealEstateQueries.UpdateRealEstate(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteRealEstate(int id)
        {
            int result = RealEstateQueries.DeleteRealEstate(id);
            return Json(new { result = result });
        }

        public JsonResult UpdateLoan(Loans model)
        {
            int result = RealEstateQueries.UpdateLoan(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteLoan(int id)
        {
            int result = RealEstateQueries.DeleteLoan(id);
            return Json(new { result = result });
        }

        [HttpGet]
        public PartialViewResult PaymentsPerMonth(int loanId)
        {
            Loans loan = RealEstateQueries.GetLoanById(loanId);
            List<Loans> list = RealEstateQueries.GetLoanByParentId(loanId);
            List<LoanInterestTableViewModel> result = LoanProcessing.CalculatePaymentsByMonth(list, loan, false);
            return PartialView(result);
        }

        [HttpPost]
        public PartialViewResult PaymentsPerMonth(Loans loan)
        {
            Loans ln = RealEstateQueries.GetLoanById(loan.Id);
            ln.StartDate = loan.StartDate;
            ln.EndDate = loan.EndDate;
            ln.InterestRatePerYear = loan.InterestRatePerYear;
            List<Loans> list = RealEstateQueries.GetLoanByParentId(loan.Id);
            List<LoanInterestTableViewModel> result = LoanProcessing.CalculatePaymentsByMonth(list, loan, true);
            return PartialView(result);
        }
    }
}