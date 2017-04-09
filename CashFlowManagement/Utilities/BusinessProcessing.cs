using CashFlowManagement.EntityModel;
using CashFlowManagement.ViewModels.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Utilities
{
    public class BusinessProcessing
    {
        public static BusinessViewModel GetBusinessViewModel(BusinessIncomes business)
        {
            BusinessViewModel viewmodel = new BusinessViewModel();

            viewmodel.Business = business;
            viewmodel.Period = RealEstateProcessing.CalculateTimePeriod(business.StartDate, business.EndDate);

            return viewmodel;
        }
    }
}