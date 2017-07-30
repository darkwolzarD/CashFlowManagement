using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Linq;

namespace CashFlowManagement.Queries
{
    public class OtherAssetQueries
    {
        public static bool CheckExistOtherAsset(string username, string assetName)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username) && x.AssetName.Equals(assetName)
                                        && x.AssetType == (int)Constants.Constants.ASSET_TYPE.OTHERS
                                        && !x.DisabledDate.HasValue).Any();
        }

        public static OtherAssetListViewModel GetOtherAssetByUser(string username)
        {
            Entities entities = new Entities();
            OtherAssetListViewModel result = new OtherAssetListViewModel();
            DateTime current = DateTime.Now;

            var otherAssets = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.OTHERS
                                                      && !x.DisabledDate.HasValue);

            foreach (var otherAsset in otherAssets)
            {
                OtherAssetViewModel otherAssetViewModel = new OtherAssetViewModel();
                otherAssetViewModel.Id = otherAsset.Id;
                otherAssetViewModel.Name = otherAsset.AssetName;
                otherAssetViewModel.Value = otherAsset.Value;
                if (otherAsset.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    otherAssetViewModel.Income = otherAsset.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    otherAssetViewModel.Income = 0;
                }
                otherAssetViewModel.AnnualIncome = otherAssetViewModel.Income * 12;
                otherAssetViewModel.RentYield = otherAssetViewModel.Income / otherAssetViewModel.Value;

                foreach (var liability in otherAsset.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    OtherAssetLiabilityViewModel liabilityViewModel = OtherAssetLiabilityQueries.CreateViewModel(liability);
                    otherAssetViewModel.Liabilities.Add(liabilityViewModel);
                }

                var liabilities = otherAssetViewModel.Liabilities.Where(x => x.StartDate <= current && x.EndDate >= current);
                otherAssetViewModel.TotalLiabilityValue = liabilities.Select(x => x.Value.Value).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalOriginalPayment = liabilities.Select(x => x.MonthlyOriginalPayment).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalInterestPayment = liabilities.Select(x => x.MonthlyInterestPayment).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalMonthlyPayment = liabilities.Select(x => x.TotalMonthlyPayment).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalPayment = liabilities.Select(x => x.TotalPayment).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalRemainedValue = liabilities.Select(x => x.RemainedValue).DefaultIfEmpty(0).Sum();
                otherAssetViewModel.TotalInterestRate = otherAssetViewModel.TotalLiabilityValue > 0 ? liabilities.Select(x => x.OriginalInterestPayment).DefaultIfEmpty(0).Sum() / otherAssetViewModel.TotalLiabilityValue * 12 : 0;
                otherAssetViewModel.RowSpan = otherAssetViewModel.Liabilities.Any() ? otherAssetViewModel.Liabilities.Count() + 3 : 2;

                result.Assets.Add(otherAssetViewModel);
            }

            result.TotalValue = result.Assets.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            result.TotalMonthlyIncome = result.Assets.Select(x => x.Income).DefaultIfEmpty(0).Sum();
            result.TotalAnnualIncome = result.TotalMonthlyIncome * 12;
            result.TotalRentYield = result.TotalMonthlyIncome / result.TotalValue;
            result.IsInitialized = UserQueries.IsCompleteInitialized(username);

            return result;
        }

        public static OtherAssetSummaryListViewModel GetOtherAssetSummaryByUser(string username)
        {
            Entities entities = new Entities();
            OtherAssetSummaryListViewModel result = new OtherAssetSummaryListViewModel();
            DateTime current = DateTime.Now;

            var otherAssets = entities.Assets.Include("Incomes").Include("Liabilities").Where(x => x.Username.Equals(username)
                                                      && x.AssetType == (int)Constants.Constants.ASSET_TYPE.OTHERS
                                                      && !x.DisabledDate.HasValue);

            foreach (var otherAsset in otherAssets)
            {
                OtherAssetSummaryViewModel otherAssetViewModel = new OtherAssetSummaryViewModel();
                otherAssetViewModel.Name = otherAsset.AssetName;
                otherAssetViewModel.Value = otherAsset.Value;
                if (otherAsset.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
                {
                    otherAssetViewModel.Income = otherAsset.Incomes1.FirstOrDefault().Value;
                }
                else
                {
                    otherAssetViewModel.Income = 0;
                }
                otherAssetViewModel.AnnualIncome = otherAssetViewModel.Income * 12;
                otherAssetViewModel.RentYield = otherAssetViewModel.Income / otherAssetViewModel.Value;

                foreach (var liability in otherAsset.Liabilities.Where(x => !x.DisabledDate.HasValue))
                {
                    OtherAssetLiabilityViewModel liabilityViewModel = OtherAssetLiabilityQueries.CreateViewModel(liability);
                    if (liabilityViewModel.StartDate <= current && liabilityViewModel.EndDate >= current)
                    {
                        otherAssetViewModel.LiabilityValue += liabilityViewModel.Value.Value;
                        otherAssetViewModel.InterestRatePerX += liabilityViewModel.InterestRatePerX;
                        otherAssetViewModel.OriginalInterestPayment += liabilityViewModel.OriginalInterestPayment;
                        otherAssetViewModel.MonthlyInterestPayment += liabilityViewModel.MonthlyInterestPayment;
                        otherAssetViewModel.MonthlyPayment += liabilityViewModel.TotalMonthlyPayment;
                        otherAssetViewModel.AnnualPayment += liabilityViewModel.TotalPayment;
                        otherAssetViewModel.RemainedValue += liabilityViewModel.RemainedValue;
                    }
                }
                otherAssetViewModel.InterestRate = otherAssetViewModel.LiabilityValue > 0 ? otherAssetViewModel.OriginalInterestPayment / otherAssetViewModel.LiabilityValue * 12: 0;
                result.OtherAssetSummaries.Add(otherAssetViewModel);
            }

            result.TotalIncome = result.OtherAssetSummaries.Sum(x => x.Income);
            result.TotalAnnualIncome = result.OtherAssetSummaries.Sum(x => x.AnnualIncome);
            result.TotalValue = result.OtherAssetSummaries.Sum(x => x.Value);
            result.TotalRentYield = result.OtherAssetSummaries.Sum(x => x.Income) / result.TotalValue;
            result.TotalLiabilityValue = result.OtherAssetSummaries.Sum(x => x.LiabilityValue);
            result.TotalInterestRate = result.TotalLiabilityValue > 0 ? result.OtherAssetSummaries.Sum(x => x.OriginalInterestPayment) / result.TotalLiabilityValue * 12 : 0;
            result.TotalMonthlyPayment = result.OtherAssetSummaries.Sum(x => x.MonthlyPayment);
            result.TotalAnnualPayment = result.OtherAssetSummaries.Sum(x => x.AnnualPayment);
            result.TotalRemainedValue = result.OtherAssetSummaries.Sum(x => x.RemainedValue);

            return result;
        }

        public static OtherAssetUpdateViewModel GetOtherAssetById(int id)
        {
            OtherAssetUpdateViewModel viewmodel = new OtherAssetUpdateViewModel();
            Entities entities = new Entities();
            var otherAsset = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            viewmodel.Id = otherAsset.Id;
            viewmodel.Name = otherAsset.AssetName;
            viewmodel.Value = otherAsset.Value;
            if (otherAsset.Incomes1.Where(x => !x.DisabledDate.HasValue).Any())
            {
                viewmodel.Income = otherAsset.Incomes1.FirstOrDefault().Value;
            }
            else
            {
                viewmodel.Income = 0;
            }
            return viewmodel;
        }

        public static double GetOtherAssetValue(int id)
        {
            Entities entities = new Entities();
            var otherAsset = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            return otherAsset.Value;
        }

        public static int CreateOtherAsset(OtherAssetCreateViewModel model, string username)
        {
            int result = 0;
            DateTime current = DateTime.Now;
            Entities entities = new Entities();

            //Create otherAsset
            Assets otherAsset = new Assets();
            otherAsset.AssetName = model.Name;
            otherAsset.Value = model.Value.Value;
            otherAsset.StartDate = current;
            otherAsset.CreatedDate = current;
            otherAsset.CreatedBy = Constants.Constants.USER;
            otherAsset.AssetType = (int)Constants.Constants.ASSET_TYPE.OTHERS;
            otherAsset.ObtainedBy = (int)Constants.Constants.OBTAIN_BY.CREATE;
            otherAsset.Username = username;

            if (model.Income.HasValue && model.Income.Value > 0)
            {
                //Create income
                Incomes income = new Incomes();
                income.Name = "Thu nhập từ " + otherAsset.AssetName;
                income.Value = model.Income.Value;
                income.IncomeDay = 1;
                income.StartDate = current;
                income.CreatedDate = current;
                income.CreatedBy = Constants.Constants.USER;
                income.IncomeType = (int)Constants.Constants.INCOME_TYPE.OTHER_ASSET_INCOME;
                income.Username = username;
                otherAsset.Incomes1.Add(income);
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
                        liability.LiabilityType = (int)Constants.Constants.LIABILITY_TYPE.OTHERS;
                        liability.CreatedDate = current;
                        liability.CreatedBy = Constants.Constants.USER;
                        liability.Username = username;
                        otherAsset.Liabilities.Add(liability);
                    }
                }
            }

            entities.Assets.Add(otherAsset);
            result = entities.SaveChanges();
            return result;
        }

        public static int UpdateOtherAsset(OtherAssetUpdateViewModel model)
        {
            Entities entities = new Entities();
            var otherAsset = entities.Assets.Where(x => x.Id == model.Id).FirstOrDefault();
            otherAsset.AssetName = model.Name;
            otherAsset.Value = model.Value.Value;

            if (entities.Incomes.Where(x => x.AssetId == model.Id).Any())
            {
                var income = entities.Incomes.Where(x => x.AssetId == model.Id).FirstOrDefault();
                income.Value = model.Income.HasValue ? model.Income.Value : 0;
                income.Name = "Thu nhập từ " + model.Name;
                entities.Incomes.Attach(income);
                entities.Entry(income).State = System.Data.Entity.EntityState.Modified;
            }

            return entities.SaveChanges();
        }

        public static int DeleteOtherAsset(int id)
        {
            DateTime current = DateTime.Now;
            Entities entities = new Entities();
            var otherAsset = entities.Assets.Where(x => x.Id == id).FirstOrDefault();
            otherAsset.DisabledDate = current;
            otherAsset.DisabledBy = Constants.Constants.USER;
            entities.Assets.Attach(otherAsset);
            entities.Entry(otherAsset).State = System.Data.Entity.EntityState.Modified;

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