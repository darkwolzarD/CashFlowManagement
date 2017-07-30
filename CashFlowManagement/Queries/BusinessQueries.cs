using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashFlowManagement.Constants;

namespace CashFlowManagement.Queries
{
    public class BusinessQueries
    {
        public static bool CheckExistBusiness(string username, string businessName)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username) && x.AssetName.Equals(businessName)
                                        && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BUSINESS
                                        && !x.DisabledDate.HasValue).Any();
        }

        public static BusinessListViewModel GetBusinessByUser(string username)
        {
            Entities entities = new Entities();
            BusinessListViewModel result = new BusinessListViewModel();
            DateTime current = DateTime.Now;

            var businesss = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BUSINESS
                                                      && !x.DisabledDate.HasValue);

            foreach (var business in businesss)
            {
                BusinessViewModel businessViewModel = new BusinessViewModel();
                businessViewModel.Id = business.Id;
                businessViewModel.Name = business.AssetName;
                businessViewModel.Value = business.Value;
                if (business.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    businessViewModel.Income = business.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    businessViewModel.Income = 0;
                }
                businessViewModel.AnnualIncome = businessViewModel.Income * 12;
                businessViewModel.RentYield = businessViewModel.Value > 0 ? businessViewModel.AnnualIncome / businessViewModel.Value : 0;

                foreach (var liability in business.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    BusinessLiabilityViewModel liabilityViewModel = BusinessLiabilityQueries.CreateViewModel(liability);
                    businessViewModel.Liabilities.Add(liabilityViewModel);
                }

                var liabilities = businessViewModel.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
                businessViewModel.TotalLiabilityValue = liabilities.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalOriginalPayment = liabilities.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalInterestPayment = liabilities.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalMonthlyPayment = liabilities.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalPayment = liabilities.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalRemainedValue = liabilities.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                businessViewModel.TotalInterestRate = businessViewModel.TotalLiabilityValue > 0 ? liabilities.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / businessViewModel.TotalLiabilityValue * 12 : 0;
                businessViewModel.RowSpan = businessViewModel.Liabilities.Any() ? businessViewModel.Liabilities.Count() + 3 : 2;

                result.Businesses.Add(businessViewModel);
            }

            result.TotalValue = result.Businesses.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            result.TotalMonthlyIncome = result.Businesses.Select(x => x.Income).DefaultIfEmpty(0).Sum();
            result.TotalAnnualIncome = result.TotalMonthlyIncome * 12;
            result.TotalRentYield = result.TotalValue > 0 ? result.TotalAnnualIncome / result.TotalValue : 0;
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static BusinessSummaryListViewModel GetBusinessSummaryByUser(string username)
        {
            Entities entities = new Entities();
            BusinessSummaryListViewModel result = new BusinessSummaryListViewModel();
            DateTime current = DateTime.Now;

            var businesss = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.BUSINESS
                                                      && !x.DisabledDate.HasValue);

            foreach (var business in businesss)
            {
                BusinessSummaryViewModel businessViewModel = new BusinessSummaryViewModel();
                businessViewModel.Name = business.AssetName;
                businessViewModel.Value = business.Value;
                if (business.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    businessViewModel.Income = business.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    businessViewModel.Income = 0;
                }
                businessViewModel.AnnualIncome = businessViewModel.Income * 12;
                businessViewModel.RentYield = businessViewModel.Value > 0 ? businessViewModel.AnnualIncome / businessViewModel.Value : 0;

                foreach (var liability in business.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    BusinessLiabilityViewModel liabilityViewModel = BusinessLiabilityQueries.CreateViewModel(liability);
                    if (liabilityViewModel.StartDate <= current && liabilityViewModel.EndDate >= current)
                    {
                        businessViewModel.LiabilityValue += liabilityViewModel.Value.Value;
                        businessViewModel.InterestRate += liabilityViewModel.InterestRate.Value;
                        businessViewModel.OriginalInterestPayment += liabilityViewModel.OriginalInterestPayment;
                        businessViewModel.MonthlyInterestPayment += liabilityViewModel.MonthlyInterestPayment;
                        businessViewModel.MonthlyPayment += liabilityViewModel.TotalMonthlyPayment;
                        businessViewModel.AnnualPayment += liabilityViewModel.TotalPayment;
                        businessViewModel.RemainedValue += liabilityViewModel.RemainedValue;
                    }   
                }
                result.BusinessSummaries.Add(businessViewModel);
            }

            result.TotalIncome = result.BusinessSummaries.Sum(x => x.Income);
            result.TotalAnnualIncome = result.BusinessSummaries.Sum(x => x.AnnualIncome);
            result.TotalValue = result.BusinessSummaries.Sum(x => x.Value);
            result.TotalRentYield = result.TotalValue > 0 ? result.BusinessSummaries.Sum(x => x.AnnualIncome) / result.TotalValue : 0;
            result.TotalLiabilityValue = result.BusinessSummaries.Sum(x => x.LiabilityValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.BusinessSummaries.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            result.TotalMonthlyPayment = result.BusinessSummaries.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.BusinessSummaries.Sum(x => x.AnnualPayment);
            result.TotalRemainedValue = result.BusinessSummaries.Sum(x => x.RemainedValue);

            return result;
        }

        public static BusinessUpdateViewModel GetBusinessById(int id)
        {
            BusinessUpdateViewModel viewmodel = new BusinessUpdateViewModel();
            Entities entities = new Entities();
            var business = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            viewmodel.Id = business.Id;
            viewmodel.Name = business.AssetName;
            viewmodel.Value = business.Value;
            if (business.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
            {
                viewmodel.Income = business.Incomes1.FirstOrDefault().Value;
            }
            else
            {
                viewmodel.Income = 0;
            }
            return viewmodel;
        }

        public static double GetBusinessValue(int id)
        {
            Entities entities = new Entities();
            var business = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            return business.Value;
        }

        public static int CreateBusiness(BusinessCreateViewModel model, string username)
        {
            int result = 0;
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            //Create business
            Assets business = new Assets();
            business.AssetName = model.Name;
            business.Value = model.Value.Value;
            business.StartDate = current;
            business.CreatedDate = current;
            business.CreatedBy = Constants.Constants.USER;
            business.AssetType = (int)Constants.Constants.ASSET_TYPE.BUSINESS;
            business.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.CREATE;
            business.Username = username;

            if (model.Income.HasValue && model.Income.Value > 0)
            {
                //Create income
                Incomes income = new Incomes();
                income.Name = "Thu nhập kinh doanh từ " + business.AssetName;
                income.Value = model.Income.Value;
                income.IncomeDay = 1;
                income.StartDate = current;
                income.CreatedDate = current;
                income.CreatedBy = Constants.Constants.USER;
                income.IncomeType = (int)Constants.Constants.INCOME_TYPE.BUSINESS_INCOME;
                income.Username = username;
                business.Incomes1.Add(income);
            }

            if (model.IsInDebt)
            {
                if (model.Liabilities != null && model.Liabilities.Liabilities.Count > 0)
                {
                    foreach (var liabilityViewModel in model.Liabilities.Liabilities)
                    {
                        Liabilities liability = new Liabilities();
                        liability.Name = liabilityViewModel.Source;
                        liability.Value = liabilityViewModel.Value.Value;
                        liability.InterestType = liabilityViewModel.InterestType;
                        liability.InterestRate = liabilityViewModel.InterestRate.Value;
                        liability.InterestRatePerX = liabilityViewModel.InterestRatePerX;
                        liability.StartDate = liabilityViewModel.StartDate.Value;
                        liability.EndDate = liabilityViewModel.EndDate.Value;
                        liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.BUSINESS;
                        liability.CreatedDate = current;
                        liability.CreatedBy = Constants.Constants.USER;
                        liability.Username = username;
                        business.Liabilities.Add(liability);
                    }
                }
            }

            entities.Assets.Add(business);
            result = entities.SaveChanges();
            return result;
        }

        public static int UpdateBusiness(BusinessUpdateViewModel model)
        {
            Entities entities = new Entities();
            var business = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();
            business.AssetName = model.Name;
            business.Value = model.Value.Value;
            
            if(entities.Incomes.Where(x => x.AssetId == model.Id).Any())
            {
                var income = entities.Incomes.Where(x => x.AssetId == model.Id).FirstOrDefault();
                income.Value = model.Income.HasValue ? model.Income.Value : 0;
                income.Name = "Thu nhập kinh doanh từ " + model.Name;
                entities.Incomes.Attach(income);
                entities.Entry(income).State = System.Data.Entity.EntityState.Modified;
            }

            return entities.SaveChanges();
        }

        public static int DeleteBusiness(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var business = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            business.DisabledDate = current;
            business.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(business);
            entities.Entry(business).State = System.Data.Entity.EntityState.Modified;

            foreach (var income in entities.Incomes.Where(x => x.AssetId == id && !x.DisabledDate.HasValue))
            {
                income.DisabledDate = current;
                income.DisabledBy = Constants.Constants.USER;
                entities.Incomes.Attach(income);
                entities.Entry(income).State = System.Data.Entity.EntityState.Modified;
            }

            foreach (var liability in entities.Liabilities.Where(x => x.AssetId == id && !x.DisabledDate.HasValue))
            {
                liability.DisabledDate = current;
                liability.DisabledBy = Constants.Constants.USER;
                entities.Liabilities.Attach(liability);
                entities.Entry(liability).State = System.Data.Entity.EntityState.Modified;
            }

            return entities.SaveChanges();
        }
    }
}