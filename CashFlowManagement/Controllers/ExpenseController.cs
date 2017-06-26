using CashFlowManagement.EntityModel;
using CashFlowManagement.Queries;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class ExpenseController : Controller
    {
        // GET: Expense
        public ActionResult Initialize(int type)
        {
            return View(type);
        }
        public ActionResult Index(int type)
        {
            return View(type);
        }

        public ActionResult _InitializeConfirmation()
        {
            return PartialView();
        }

        public ActionResult _Report(int type)
        {
            ExpenseListViewModel model = ExpenseQueries.GetExpenseByUser(UserQueries.GetCurrentUsername(), type);
            return PartialView(model);
        }

        public PartialViewResult _ExpenseForm(int? id, int type)
        {
            if (id > 0)
            {
                Expenses model = ExpenseQueries.GetExpenseById(id.Value);
                return PartialView(model);
            }
            else
            {
                Expenses model = new Expenses
                {
                    Id = 0,
                    ExpenseType = type
                };
                return PartialView(model);
            }
        }
        public PartialViewResult _ExpenseInitializeForm(int? id, int type)
        {
            if (id > 0)
            {
                Expenses model = ExpenseQueries.GetExpenseById(id.Value);
                return PartialView(model);
            }
            else
            {
                Expenses model = new Expenses
                {
                    Id = 0,
                    ExpenseType = type
                };
                return PartialView(model);
            }
        }

        public PartialViewResult _ExpenseTable(int type)
        {
            ExpenseListViewModel model = ExpenseQueries.GetExpenseByUser(UserQueries.GetCurrentUsername(), type);
            return PartialView(model);
        }

        public ActionResult _CashflowDetail(Expenses Expense, int? ExpenseId)
        {
            if (Expense.Id == 0 && (ExpenseId == 0 || !ExpenseId.HasValue))
            {
                Entities entities = new Entities();
                Expenses dbExpense = entities.Expenses.Where(x => x.Name.Equals(Expense.Name) && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate).FirstOrDefault();
                if (dbExpense != null && !dbExpense.EndDate.HasValue)
                {
                    return Content("-1");
                }
                else if (dbExpense != null && Expense.StartDate <= dbExpense.EndDate)
                {
                    return Content("-2");
                }
            }
            CashFlowDetailListViewModel model = ExpenseQueries.GetCashFlowDetail(Expense, ExpenseId, UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public PartialViewResult _ExpenseUpdateModal(int ExpenseId)
        {
            Expenses model = ExpenseQueries.GetExpenseById(ExpenseId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ProcessExpense(Expenses model)
        {
            int result = -1;
            if (ModelState.IsValid)
            {
                int type = model.ExpenseType;
                if (model.Id == 0)
                {
                    result = ExpenseQueries.CreateExpense(model, type, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Tạo chi tiêu thành công");
                    }
                    else if (result == -1)
                    {
                        return Content("Xin kết thúc giai đoạn trước trước khi tạo giai đoạn mới");
                    }
                    else if (result == -2)
                    {
                        return Content("Thu nhập đã tồn tại trước đó");
                    }
                    else
                    {
                        return Content("Có lỗi xảy ra");
                    }
                }
                else
                {
                    result = ExpenseQueries.UpdateExpense(model, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Cập nhật chi tiêu thành công");
                    }
                    else
                    {
                        return Content("Có lỗi xảy ra");
                    }
                }
            }
            else
            {
                return Content("Vui lòng nhập đúng thông tin thu nhập!");
            }
        }

        public ActionResult DeleteExpense(int expenseId)
        {
            int result = ExpenseQueries.DeleteExpense(expenseId);
            if (result > 0)
            {
                return Content("Xóa chi tiêu thành công");
            }
            else
            {
                return Content("Có lỗi xảy ra");
            }
        }

        public ActionResult GetStartDate(string name, int type)
        {
            string datetime = ExpenseQueries.GetStartDate(name, UserQueries.GetCurrentUsername(), type);
            return Content(datetime);

        }
    }
}