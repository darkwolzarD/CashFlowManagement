using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Constants
{
    public static class Constants
    {
        public enum ASSET_TYPE { AVAILABLE_MONEY = 1, BUSINESS = 2, BANK_DEPOSIT = 3, REAL_ESTATE = 4, STOCK = 5, INSURANCE = 6, OTHERS = 7 }
        public enum INCOME_TYPE { SALARY_INCOME = 1, BUSINESS_INCOME = 2, BANK_DEPOSIT_INCOME = 3, REAL_ESTATE_INCOME = 4, STOCK_INCOME = 5 }
        public enum LIABILITY_TYPE { REAL_ESTATE = 1, CAR = 2, CREDIT_CARD = 3, BUSINESS = 4, STOCK = 5, OTHERS = 6 }
        public enum EXPENSE_TYPE { REAL_ESTATE = 1, CAR = 2, CREDIT_CARD = 3, BUSINESS = 4, STOCK = 5, LIABILITY = 6, FAMILY = 7, OTHERS = 8 }
        public enum LOG_TYPE { EXPENSE = 1, INCOME = 2, SELL = 3, BUY = 4, ADD = 5, UPDATE = 6, DELETE = 7 }
        public enum LOG_FILTER_TYPE { INCOME_EXPENSE = 1, OTHERS = 2}
        public enum OBTAIN_BY { CREATE = 1, BUY = 2}
        public static string USER = "user";
        public static string SYSTEM = "system";
    }
}