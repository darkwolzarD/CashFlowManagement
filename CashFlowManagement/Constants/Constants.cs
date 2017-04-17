using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Constants
{
    public static class Constants
    {
        public enum ASSET_TYPE { AVAILABLE_MONEY = 1, BUSINESS = 2, BANK_DEPOSIT = 3, REAL_ESTATE = 4, STOCK = 5 }
        public enum INCOME_TYPE { SALARY_INCOME = 1, BUSINESS_INCOME = 2, BANK_DEPOSIT_INCOME = 3, REAL_ESTATE_INCOME = 4, STOCK_INCOME = 5 }
        public enum LIABILITY_TYPE { REAL_ESTATE = 1, STOCK = 2, HOME = 3, CAR = 4, CREDIT_CARD = 5, BANK = 6, OTHERS = 7 }
        public enum EXPENSE_TYPE { HOME = 1, CAR = 2, CREDIT_CARD = 3, BANK = 4, FAMILY = 5, OTHERS = 6 }
        public static string USER = "user";
        public static string SYSTEM = "system";
    }
}