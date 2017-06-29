using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashFlowManagement.Constants;

namespace CashFlowManagement.Queries
{
    public class RealEstateQueries
    {
        public static RealEstateListViewModel GetRealEstateByUser(string username)
        {
            Entities entities = new Entities();
            RealEstateListViewModel result = new RealEstateListViewModel();

            var realEstates = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE
                                                      && !x.DisabledDate.HasValue);

            foreach (var realEstate in realEstates)
            {
                RealEstateViewModel realEstateViewModel = new RealEstateViewModel();
                realEstateViewModel.Name = realEstate.AssetName;
                realEstateViewModel.Value = realEstate.Value;
                realEstateViewModel.Income = realEstate.Incomes1.FirstOrDefault().Value;
                realEstateViewModel.AnnualIncome = realEstateViewModel.Income * 12;
                realEstateViewModel.RentYield = realEstateViewModel.Income / realEstateViewModel.Value;

                foreach (var liability in realEstate.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    RealEstateLiabilityViewModel liabilityViewModel = RealEstateLiabilityQueries.CreateViewModel(liability);
                    realEstateViewModel.Liabilities.Add(liabilityViewModel);
                }

                realEstateViewModel.TotalLiabilityValue = realEstateViewModel.Liabilities.Select(x => x.Value).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalOriginalPayment = realEstateViewModel.Liabilities.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalInterestPayment = realEstateViewModel.Liabilities.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalMonthlyPayment = realEstateViewModel.Liabilities.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalPayment = realEstateViewModel.Liabilities.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalRemainedValue = realEstateViewModel.Liabilities.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalInterestRate = realEstateViewModel.TotalInterestPayment / realEstateViewModel.TotalLiabilityValue * 12;
                realEstateViewModel.RowSpan = realEstateViewModel.Liabilities.Count() + 3;

                result.RealEstates.Add(realEstateViewModel);
            }

            result.TotalValue = result.RealEstates.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            result.TotalMonthlyIncome = result.RealEstates.Select(x => x.Income).DefaultIfEmpty(0).Sum();
            result.TotalAnnualIncome = result.TotalMonthlyIncome * 12;
            result.TotalRentYield = result.TotalMonthlyIncome / result.TotalValue;

            return result;
        }
    }
}