using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace CashFlowManagement.Utilities
{
    public class DoubleModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result != null && !string.IsNullOrEmpty(result.AttemptedValue))
            {
                if (bindingContext.ModelType == typeof(double))
                {
                    double temp;
                    if (double.TryParse(
                        result.AttemptedValue,
                        NumberStyles.Number,
                        CultureInfo.GetCultureInfo("vi-VN"),
                        out temp)
                    )
                    {
                        return temp;
                    }
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}