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
    public class IncomeController : Controller
    {
        // GET: Income
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize()
        {
            return View();
        }

        public ActionResult _InitializeConfirmation()
        {
            IncomeListViewModel model = IncomeQueries.GetIncomeByUser(UserQueries.GetCurrentUsername(), (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME);
            return PartialView(model);
        }

        public PartialViewResult _IncomeForm(int? id)
        {
            if(id > 0)
            {
                Incomes model = IncomeQueries.GetIncomeById(id.Value);
                return PartialView(model);
            }
            else
            {
                Incomes model = new Incomes
                {
                    Id = 0,
                    IncomeType = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME
                };
                return PartialView(model);
            }
        }

        public PartialViewResult _IncomeInitializeForm(int? id)
        {
            if (id > 0)
            {
                Incomes model = IncomeQueries.GetIncomeById(id.Value);
                return PartialView(model);
            }
            else
            {
                Incomes model = new Incomes
                {
                    Id = 0,
                    IncomeType = (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME
                };
                return PartialView(model);
            }
        }

        public PartialViewResult _IncomeTable()
        {
            IncomeListViewModel model = IncomeQueries.GetIncomeByUser(UserQueries.GetCurrentUsername(), (int)Constants.Constants.INCOME_TYPE.SALARY_INCOME);
            return PartialView(model);
        }

        public ActionResult _CashflowDetail(Incomes income, int? incomeId)
        {
            if (income.Id == 0 && (incomeId == 0 || !incomeId.HasValue))
            {
                Entities entities = new Entities();
                string username = UserQueries.GetCurrentUsername();
                Incomes dbIncome = entities.Incomes.Where(x => x.Username.Equals(username) && x.Name.Equals(income.Name) && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate).FirstOrDefault();
                if (dbIncome != null && !dbIncome.EndDate.HasValue)
                {
                    return Content("-1");
                }
                else if (dbIncome != null && income.StartDate <= dbIncome.EndDate)
                {
                    return Content("-2");
                }
            }
            CashFlowDetailListViewModel model = IncomeQueries.GetCashFlowDetail(income, incomeId, UserQueries.GetCurrentUsername());
            return PartialView(model);
        }

        public PartialViewResult _IncomeUpdateModal(int incomeId)
        {
            Incomes model = IncomeQueries.GetIncomeById(incomeId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ProcessIncome(Incomes model)
        {
            int result = -1;
            if (ModelState.IsValid)
            {
                int type = model.IncomeType;
                if (model.Id == 0)
                {
                    result = IncomeQueries.CreateIncome(model, type, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Tạo thu nhập thành công");
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
                    result = IncomeQueries.UpdateIncome(model, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Cập nhật thu nhập thành công");
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

        [HttpPost]
        public ActionResult InitializeIncome(Incomes model)
        {
            int result = -1;
            if (ModelState.IsValid)
            {
                int type = model.IncomeType;
                if (model.Id == 0)
                {
                    result = IncomeQueries.InitializeIncome(model, type, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Tạo thu nhập thành công");
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
                    result = IncomeQueries.UpdateInitializeIncome(model, UserQueries.GetCurrentUsername());
                    if (result > 0)
                    {
                        return Content("Cập nhật thu nhập thành công");
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

        public ActionResult DeleteIncome(int incomeId)
        {
            int result = IncomeQueries.DeleteIncome(incomeId);
            if (result > 0)
            {
                return Content("Xóa thu nhập thành công");
            }
            else
            {
                return Content("Có lỗi xảy ra");
            }
        }

        public ActionResult DeleteInitializeIncome(int incomeId)
        {
            int result = IncomeQueries.DeleteIncome(incomeId);
            if (result > 0)
            {
                return Content("Xóa thu nhập thành công");
            }
            else
            {
                return Content("Có lỗi xảy ra");
            }
        }

        public ActionResult GetStartDate(string name)
        {
            string datetime = IncomeQueries.GetStartDate(name, UserQueries.GetCurrentUsername());
            return Content(datetime);

        }
    }
}