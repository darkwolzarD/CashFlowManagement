using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Utilities
{
    public class FormatUtility
    {
        public static string DisplayThousandSeparatorsForNumber(double number)
        {
            return string.Format(new CultureInfo("vi-VN"), "{0:N0}", number);
        }

        public static string DisplayPercentageForNumber(double number)
        {
            return string.Format(new CultureInfo("vi-VN"), "{0:N2}", number) + "%";
        }

        public static int CalculateTimePeriod(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
        }
    }
}