using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashFlowManagement.Constants
{
    public static class Constants
    {
        public enum ASSET_TYPE { AVAILABLE_MONEY = 1, BUSINESS = 2, BANK_DEPOSIT = 3, REAL_ESTATE = 4, STOCK = 5, INSURANCE = 6, OTHERS = 7 }
        public enum INCOME_TYPE { SALARY_INCOME = 1, BUSINESS_INCOME = 2, BANK_DEPOSIT_INCOME = 3, REAL_ESTATE_INCOME = 4, STOCK_INCOME = 5 }
        public enum LIABILITY_TYPE { REAL_ESTATE = 1, CAR = 2, CREDIT_CARD = 3, BUSINESS = 4, STOCK = 5, OTHERS = 6, INSURANCE = 7 }
        public enum EXPENSE_TYPE { REAL_ESTATE = 1, CAR = 2, CREDIT_CARD = 3, BUSINESS = 4, STOCK = 5, INSURANCE = 6, FAMILY = 7, OTHERS = 8, LIABILITY_OTHERS = 9  }
        public enum LOG_TYPE { EXPENSE = 1, INCOME = 2, SELL = 3, BUY = 4, ADD = 5, UPDATE = 6, DELETE = 7, WITHDRAW = 8 }
        public enum HISTORY_TYPE { EXPENSE = 1, INCOME = 2, SELL = 3, BUY = 4, CREATE = 5, UPDATE = 6, DELETE = 7, WITHDRAW = 8 }
        public enum LOG_FILTER_TYPE { INCOME_EXPENSE = 1, OTHERS = 2, DIVIDEND = 3}
        public enum TRANSACTION_TYPE {CREATE = 1, BUY = 2, SELL = 3, DIVIDEND = 4 }
        public enum INTEREST_OBTAIN_TYPE { ORIGIN = 1, START = 2, END = 3 }
        public enum OBTAIN_BY { CREATE = 1, BUY = 2, INCOME = 3, EXPENSE = 4}
        public enum DIVIDEND_TYPE { MONEY = 1, STOCK = 2 }
        public enum INTEREST_TYPE { FIXED = 1, REDUCED = 2 }
        public enum INCOME_ERROR { INVALID_START_DATE = -1, INVALID_END_DATE = -2, INVALID_NAME = -3 }
        public enum EXPENSE_ERROR { INVALID_START_DATE = -1, INVALID_END_DATE = -2, INVALID_NAME = -3 }
        public enum OBJECT_TYPE { ASSET = 1, INCOME = 2, LIABILITY = 3, EXPENSE = 4 }
        public enum INTEREST_RATE_PER { MONTH = 1, YEAR = 2 }

        public static string USER = "user";
        public static string SYSTEM = "system";
        public static List<SelectListItem> SexList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem{ Text = "Nam", Value = "1", Selected = true });
            list.Add(new SelectListItem { Text = "Nữ", Value = "2" });
            return list;
        }

        public static List<SelectListItem> InterestType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Cố định", Value = ((int)Constants.INTEREST_TYPE.FIXED).ToString(), Selected = true });
            list.Add(new SelectListItem { Text = "Giảm dần", Value = ((int)Constants.INTEREST_TYPE.REDUCED).ToString() });
            return list;
        }

        public static List<SelectListItem> InterestPerX()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Tháng", Value = ((int)Constants.INTEREST_RATE_PER.MONTH).ToString() });
            list.Add(new SelectListItem { Text = "Năm", Value = ((int)Constants.INTEREST_RATE_PER.YEAR).ToString(), Selected = true });
            return list;
        }

        public static List<SelectListItem> IncomeDayList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 1; i <= 28; i++)
            {
                list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return list;
        }
    }
}