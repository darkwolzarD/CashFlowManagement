using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels.Business;
using CashFlowManagement.Utilities;

namespace CashFlowManagement.Controllers
{
    public class BusinessController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult BusinessTable()
        {
            List<BusinessIncomes> ListBusinessIncomes = BusinessQueries.GetBusinessByUser("test");
            BusinessListViewModel result = BusinessProcessing.GetBusinessListViewModel(ListBusinessIncomes);
            return PartialView(result);
        }

        public PartialViewResult BusinessModal()
        {
            BusinessIncomes model = new BusinessIncomes();
            return PartialView(model);
        }

        public PartialViewResult UpdateBusinessModal(int id)
        {
            BusinessIncomes model = BusinessQueries.GetBusinessById(id);
            return PartialView(model);
        }

        public PartialViewResult LoanModal(int id)
        {
            BusinessLoan model = new BusinessLoan();
            model.Id = id;
            return PartialView(model);
        }

        public PartialViewResult UpdateLoanModal(int id)
        {
            BusinessLoan model = BusinessQueries.GetLoanById(id);
            return PartialView(model);
        }

        public PartialViewResult UpdateLoanWithRateModal(int id)
        {
            BusinessLoan model = BusinessQueries.GetLoanById(id);
            return PartialView(model);
        }

        public JsonResult CreateBusiness(BusinessIncomes model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = BusinessQueries.CreateBusiness(model);
            return Json(new { result = result });
        }

        public JsonResult CreateLoan(BusinessLoan model)
        {
            DateTime current = DateTime.Now;
            model.CreatedDate = new DateTime(current.Year, current.Month, 1);
            int result = BusinessQueries.CreateLoan(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateBusiness(BusinessIncomes model)
        {
            int result = BusinessQueries.UpdateBusiness(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteBusiness(int id)
        {
            int result = BusinessQueries.DeleteBusiness(id);
            return Json(new { result = result });
        }

        public JsonResult UpdateLoan(BusinessLoan model)
        {
            int result = BusinessQueries.UpdateLoan(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteLoan(int id)
        {
            int result = BusinessQueries.DeleteLoan(id);
            return Json(new { result = result });
        }

        [HttpGet]
        public PartialViewResult PaymentsPerMonth(int loanId)
        {
            BusinessLoan loan = BusinessQueries.GetLoanById(loanId);
            List<BusinessLoan> list = BusinessQueries.GetLoanByParentId(loanId);
            List<LoanInterestTableViewModel> result = BusinessLoanProcessing.CalculatePaymentsByMonth(list, loan, false);
            return PartialView(result);
        }

        [HttpPost]
        public PartialViewResult PaymentsPerMonth(BusinessLoan loan)
        {
            BusinessLoan ln = BusinessQueries.GetLoanById(loan.Id);
            ln.StartDate = loan.StartDate;
            ln.EndDate = loan.EndDate;
            ln.InterestRatePerYear = loan.InterestRatePerYear;
            List<BusinessLoan> list = BusinessQueries.GetLoanByParentId(loan.Id);
            List<LoanInterestTableViewModel> result = BusinessLoanProcessing.CalculatePaymentsByMonth(list, loan, true);
            return PartialView(result);
        }

        public ActionResult BusinessSummary()
        {
            List<BusinessIncomes> ListBusinessIncomes = BusinessQueries.GetBusinessByUser("test");
            BusinessListViewModel result = BusinessProcessing.GetBusinessListViewModel(ListBusinessIncomes);
            return View(result);
        }
    }
}