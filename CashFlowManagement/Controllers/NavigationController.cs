using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Controllers
{
    public class NavigationController : Controller
    {
        // GET: Navigation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _NavigationForm()
        {
            NavigationListViewModel model = new NavigationListViewModel();
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 1,
                Display = "Khởi tạo thu nhập lương",
                Url = Url.Action("Index", "Salary"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 2,
                Display = "Khởi tạo bất động sản",
                Url = Url.Action("Index", "RealEstate"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 3,
                Display = "Khởi tạo kinh doanh",
                Url = Url.Action("Index", "Business"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 4,
                Display = "Khởi tạo tài khoản tiết kiệm",
                Url = Url.Action("Index", "BankDeposit"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 5,
                Display = "Khởi tạo chứng khoán",
                Url = Url.Action("Index", "Stock"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 6,
                Display = "Khởi tạo bảo hiểm nhân thọ",
                Url = Url.Action("Index", "Insurance"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 7,
                Display = "Khởi tạo tài sản khác",
                Url = Url.Action("Index", "OtherAsset"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 8,
                Display = "Khởi tạo vay xe hơi và tiêu sản khác",
                Url = Url.Action("Index", "CarLiability"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 9,
                Display = "Khởi tạo vay thẻ tín dụng",
                Url = Url.Action("Index", "CreditCardLiability"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 10,
                Display = "Khởi tạo khoản vay khác",
                Url = Url.Action("Index", "OtherLiability"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 11,
                Display = "Khởi tạo chi tiêu gia đình",
                Url = Url.Action("Index", "FamilyExpense"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 11,
                Display = "Khởi tạo chi tiêu khác",
                Url = Url.Action("Index", "OtherExpense"),
                Selected = false,
                Current = false
            });
            model.NavigationList.Add(new NavigationViewModel
            {
                Id = 11,
                Display = "Khởi tạo tiền mặt có sẵn",
                Url = Url.Action("Index", "AvailableMoney"),
                Selected = false,
                Current = false
            });
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _NavigationForm(NavigationListViewModel model)
        {
            var navList = model.NavigationList.Where(x => x.Selected == true).ToList();
            int newId = 1;
            foreach (var item in navList)
            {
                item.Id = newId;
                newId++;
            }
            navList.FirstOrDefault().Current = true;
            HttpContext.Session["NAV_LIST"] = navList;
            return Redirect(model.NavigationList.FirstOrDefault().Url);
        }

        public ActionResult Previous()
        {
            var navList = (List<NavigationViewModel>)HttpContext.Session["NAV_LIST"];
            var current = navList.Where(x => x.Current == true).FirstOrDefault();
            int id = current.Id;
            current.Current = false;
            navList.Where(x => x.Id == current.Id - 1).FirstOrDefault().Current = true;
            HttpContext.Session["NAV_LIST"] = navList;
            string url = navList.Where(x => x.Id == current.Id - 1).FirstOrDefault().Url;
            return Redirect(url);
        }

        public ActionResult Next()
        {
            var navList = (List<NavigationViewModel>)HttpContext.Session["NAV_LIST"];
            var current = navList.Where(x => x.Current == true).FirstOrDefault();
            int id = current.Id;
            current.Current = false;
            if (id + 1 < navList.Count() + 1)
            {
                navList.Where(x => x.Id == current.Id + 1).FirstOrDefault().Current = true;
                HttpContext.Session["NAV_LIST"] = navList;
                string url = navList.Where(x => x.Id == current.Id + 1).FirstOrDefault().Url;
                return Redirect(url);
            } 
            else
            {
                return RedirectToAction("Index", "FinancialStatus");
            }
        }
    }
}