﻿using CashFlowManagement.EntityModel;
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
        public static bool CheckExistRealEstate(string username, string realEstateName)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username) && x.AssetName.Equals(realEstateName)
                                        && x.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE
                                        && !x.DisabledDate.HasValue).Any();
        }

        public static RealEstateListViewModel GetRealEstateByUser(string username)
        {
            Entities entities = new Entities();
            RealEstateListViewModel result = new RealEstateListViewModel();
            DateTime current = DateTime.Now;

            var realEstates = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE
                                                      && !x.DisabledDate.HasValue);

            foreach (var realEstate in realEstates)
            {
                RealEstateViewModel realEstateViewModel = new RealEstateViewModel();
                realEstateViewModel.Id = realEstate.Id;
                realEstateViewModel.Name = realEstate.AssetName;
                realEstateViewModel.Value = realEstate.Value;
                if (realEstate.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    realEstateViewModel.Income = realEstate.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    realEstateViewModel.Income = 0;
                }
                realEstateViewModel.AnnualIncome = realEstateViewModel.Income * 12;
                realEstateViewModel.RentYield = realEstateViewModel.Value > 0 ? realEstateViewModel.AnnualIncome / realEstateViewModel.Value : 0;

                foreach (var liability in realEstate.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    RealEstateLiabilityViewModel liabilityViewModel = RealEstateLiabilityQueries.CreateViewModel(liability);
                    realEstateViewModel.Liabilities.Add(liabilityViewModel);
                }

                var liabilities = realEstateViewModel.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
                realEstateViewModel.TotalLiabilityValue = liabilities.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalOriginalPayment = liabilities.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalInterestPayment = liabilities.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalMonthlyPayment = liabilities.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalPayment = liabilities.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalRemainedValue = liabilities.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                realEstateViewModel.TotalInterestRate = realEstateViewModel.TotalLiabilityValue > 0 ? liabilities.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / realEstateViewModel.TotalLiabilityValue * 12 : 0;
                realEstateViewModel.RowSpan = realEstateViewModel.Liabilities.Any() ? realEstateViewModel.Liabilities.Count() + 3 : 2;

                result.RealEstates.Add(realEstateViewModel);
            }

            result.TotalValue = result.RealEstates.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            result.TotalMonthlyIncome = result.RealEstates.Select(x => x.Income).DefaultIfEmpty(0).Sum();
            result.TotalAnnualIncome = result.TotalMonthlyIncome * 12;
            result.TotalRentYield = result.TotalValue > 0 ? result.TotalAnnualIncome / result.TotalValue : 0;
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static RealEstateSummaryListViewModel GetRealEstateSummaryByUser(string username)
        {
            Entities entities = new Entities();
            RealEstateSummaryListViewModel result = new RealEstateSummaryListViewModel();
            DateTime current = DateTime.Now;

            var realEstates = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE
                                                      && !x.DisabledDate.HasValue);

            foreach (var realEstate in realEstates)
            {
                RealEstateSummaryViewModel realEstateViewModel = new RealEstateSummaryViewModel();
                realEstateViewModel.Name = realEstate.AssetName;
                realEstateViewModel.Value = realEstate.Value;
                if (realEstate.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    realEstateViewModel.Income = realEstate.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    realEstateViewModel.Income = 0;
                }
                realEstateViewModel.AnnualIncome = realEstateViewModel.Income * 12;
                realEstateViewModel.RentYield = realEstateViewModel.Value > 0 ? realEstateViewModel.AnnualIncome / realEstateViewModel.Value : 0;

                foreach (var liability in realEstate.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    RealEstateLiabilityViewModel liabilityViewModel = RealEstateLiabilityQueries.CreateViewModel(liability);
                    if (liabilityViewModel.StartDate <= current && liabilityViewModel.EndDate >= current)
                    {
                        realEstateViewModel.LiabilityValue += liabilityViewModel.Value.Value;
                        realEstateViewModel.OriginalInterestPayment += liabilityViewModel.OriginalInterestPayment;
                        realEstateViewModel.MonthlyInterestPayment += liabilityViewModel.MonthlyInterestPayment;
                        realEstateViewModel.MonthlyPayment += liabilityViewModel.TotalMonthlyPayment;
                        realEstateViewModel.AnnualPayment += liabilityViewModel.TotalPayment;
                        realEstateViewModel.RemainedValue += liabilityViewModel.RemainedValue;
                    }
                }
                realEstateViewModel.InterestRate = realEstateViewModel.LiabilityValue > 0 ? realEstateViewModel.OriginalInterestPayment / realEstateViewModel.LiabilityValue * 12 : 0;
                result.RealEstateSummaries.Add(realEstateViewModel);
            }

            result.TotalIncome = result.RealEstateSummaries.Sum(x => x.Income);
            result.TotalAnnualIncome = result.RealEstateSummaries.Sum(x => x.AnnualIncome);
            result.TotalValue = result.RealEstateSummaries.Sum(x => x.Value);
            result.TotalRentYield = result.TotalValue > 0 ? result.RealEstateSummaries.Sum(x => x.AnnualIncome) / result.TotalValue : 0;
            result.TotalLiabilityValue = result.RealEstateSummaries.Sum(x => x.LiabilityValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.RealEstateSummaries.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            result.TotalMonthlyPayment = result.RealEstateSummaries.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.RealEstateSummaries.Sum(x => x.AnnualPayment);
            result.TotalRemainedValue = result.RealEstateSummaries.Sum(x => x.RemainedValue);

            return result;
        }

        public static RealEstateUpdateViewModel GetRealEstateById(int id)
        {
            RealEstateUpdateViewModel viewmodel = new RealEstateUpdateViewModel();
            Entities entities = new Entities();
            var realEstate = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            viewmodel.Id = realEstate.Id;
            viewmodel.Name = realEstate.AssetName;
            viewmodel.Value = realEstate.Value;
            if (realEstate.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
            {
                viewmodel.Income = realEstate.Incomes1.FirstOrDefault().Value;
            }
            else
            {
                viewmodel.Income = 0;
            }
            return viewmodel;
        }

        public static double GetRealEstateValue(int id)
        {
            Entities entities = new Entities();
            var realEstate = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            return realEstate.Value;
        }

        public static int CreateRealEstate(RealEstateCreateViewModel model, string username)
        {
            int result = 0;
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            //Create real estate
            Assets realEstate = new Assets();
            realEstate.AssetName = model.Name;
            realEstate.Value = model.Value.Value;
            realEstate.StartDate = current;
            realEstate.CreatedDate = current;
            realEstate.CreatedBy = Constants.Constants.USER;
            realEstate.AssetType = (int)Constants.Constants.ASSET_TYPE.REAL_ESTATE;
            realEstate.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.CREATE;
            realEstate.Username = username;

            if (model.Income.HasValue && model.Income.Value > 0)
            {
                //Create rent income
                Incomes income = new Incomes();
                income.Name = "Thu nhập cho thuê từ " + realEstate.AssetName;
                income.Value = model.Income.Value;
                income.IncomeDay = 1;
                income.StartDate = current;
                income.CreatedDate = current;
                income.CreatedBy = Constants.Constants.USER;
                income.IncomeType = (int)Constants.Constants.INCOME_TYPE.REAL_ESTATE_INCOME;
                income.Username = username;
                realEstate.Incomes1.Add(income);
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
                        liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE;
                        liability.CreatedDate = current;
                        liability.CreatedBy = Constants.Constants.USER;
                        liability.Username = username;
                        realEstate.Liabilities.Add(liability);
                    }
                }
            }

            entities.Assets.Add(realEstate);
            result = entities.SaveChanges();
            return result;
        }

        public static int UpdateRealEstate(RealEstateUpdateViewModel model)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            var realEstate = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();
            realEstate.AssetName = model.Name;
            realEstate.Value = model.Value.Value;
            entities.Assets.Attach(realEstate);
            entities.Entry(realEstate).State = System.Data.Entity.EntityState.Modified;

            if (entities.Incomes.Where(x => x.AssetId == model.Id).Any())
            {
                var income = entities.Incomes.Where(x => x.AssetId == model.Id).FirstOrDefault();
                income.Value = model.Income.HasValue ? model.Income.Value : 0;
                income.Name = "Thu nhập cho thuê từ " + model.Name;
                entities.Incomes.Attach(income);
                entities.Entry(income).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                Incomes income = new Incomes();
                income.Name = "Thu nhập cho thuê từ " + realEstate.AssetName;
                income.Value = model.Income.Value;
                income.IncomeDay = 1;
                income.StartDate = current;
                income.CreatedDate = current;
                income.CreatedBy = Constants.Constants.USER;
                income.IncomeType = (int)Constants.Constants.INCOME_TYPE.REAL_ESTATE_INCOME;
                income.Username = realEstate.Username;
                realEstate.Incomes1.Add(income);
            }

            return entities.SaveChanges();
        }

        public static int DeleteRealEstate(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var realEstate = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            realEstate.DisabledDate = current;
            realEstate.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(realEstate);
            entities.Entry(realEstate).State = System.Data.Entity.EntityState.Modified;

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