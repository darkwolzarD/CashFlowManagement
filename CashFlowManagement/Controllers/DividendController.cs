using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class DividendController : Controller
    {
        // GET: Diviend
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult StockCodeTable()
        {
            List<StockCodes> model = DividendQueries.GetStockCodesByUser("test");
            return PartialView(model);
        }

        public PartialViewResult StockCodeModal()
        {
            StockCodes model = new StockCodes();
            return PartialView(model);
        }

        public PartialViewResult UpdateStockCodeModal(int id)
        {
            StockCodes model = DividendQueries.GetStockCodeById(id);
            return PartialView(model);
        }

        public PartialViewResult TransactionModal(int id)
        {
            StockTransactions model = new StockTransactions();
            model.StockId = id;
            return PartialView(model);
        }

        public PartialViewResult UpdateTransactionModal(int id)
        {
            StockTransactions model = DividendQueries.GetTransactionById(id);
            return PartialView(model);
        }

        public JsonResult CreateStockCode(StockCodes model)
        {
            model.Username = "test";
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = DividendQueries.CreateStockCode(model);
            return Json(new { result = result });
        }

        public JsonResult CreateTransaction(StockTransactions model)
        {
            DateTime current = DateTime.Now;
            model.StartDate = new DateTime(current.Year, current.Month, 1);
            int result = DividendQueries.CreateTransaction(model);
            return Json(new { result = result });
        }

        public JsonResult UpdateStockCode(StockCodes model)
        {
            int result = DividendQueries.UpdateStockCode(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteStockCode(int id)
        {
            int result = DividendQueries.DeleteStockCode(id);
            return Json(new { result = result });
        }

        public JsonResult UpdateTransaction(StockTransactions model)
        {
            int result = DividendQueries.UpdateTransaction(model);
            return Json(new { result = result });
        }

        public JsonResult DeleteTransaction(int id)
        {
            int result = DividendQueries.DeleteTransaction(id);
            return Json(new { result = result });
        }
    }
}