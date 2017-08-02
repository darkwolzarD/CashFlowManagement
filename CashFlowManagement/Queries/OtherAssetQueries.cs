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
                otherAssetViewModel.RentYield = otherAssetViewModel.AnnualIncome / otherAssetViewModel.Value;

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
                otherAssetViewModel.RentYield = otherAssetViewModel.AnnualIncome / otherAssetViewModel.Value;

                result.OtherAssetSummaries.Add(otherAssetViewModel);
            }

            result.TotalValue = result.OtherAssetSummaries.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            result.TotalIncome = result.OtherAssetSummaries.Select(x => x.Income).DefaultIfEmpty(0).Sum();
            result.TotalAnnualIncome = result.TotalIncome * 12;
            result.TotalRentYield = result.TotalAnnualIncome / result.TotalValue;

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

            Incomes income = new Incomes();
            income.Name = "Thu nhập từ " + otherAsset.AssetName;
            income.Value = model.Income.HasValue ? model.Income.Value : 0;
            income.IncomeDay = 1;
            income.StartDate = current;
            income.CreatedDate = current;
            income.CreatedBy = Constants.Constants.USER;
            income.IncomeType = (int)Constants.Constants.INCOME_TYPE.OTHER_ASSET_INCOME;
            income.Username = username;
            otherAsset.Incomes1.Add(income);

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

            var income = entities.Incomes.Where(x => x.AssetId == model.Id).FirstOrDefault();
            income.Value = model.Income.HasValue ? model.Income.Value : 0;
            income.Name = "Thu nhập từ " + model.Name;
            entities.Incomes.Attach(income);
            entities.Entry(income).State = System.Data.Entity.EntityState.Modified;

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

            return entities.SaveChanges();
        }
    }
}