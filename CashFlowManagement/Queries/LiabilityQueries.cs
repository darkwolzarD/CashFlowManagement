using CashFlowManagement.EntityModel;
using CashFlowManagement.Utilities;
using CashFlowManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CashFlowManagement.Queries
{
    public class LiabilityQueries
    {
        public static LiabilityListViewModel GetLiabilityByUser(string username, int type)
        {
            Entities entities = new Entities();
            List<Liabilities> queryResult = (from liability in entities.Liabilities
                                             where liability.Username.Equals(username) && liability.LiabilityType == type
                                             && !liability.DisabledDate.HasValue
                                             select liability).ToList();
            List<LiabilityViewModel> list = new List<LiabilityViewModel>();

            foreach (var liability in queryResult)
            {
                LiabilityViewModel model = GetLiabilityViewModelByLiability(liability);
                list.Add(model);
            }

            LiabilityListViewModel result = new LiabilityListViewModel
            {
                List = list,
                Type = type
            };
            return result;
        }

        public static Liabilities GetLiabilityById(int id)
        {
            Entities entities = new Entities();
            Liabilities result = entities.Liabilities.Where(x => x.Id == id && !x.DisabledDate.HasValue).FirstOrDefault();
            return result;
        }

        public static List<Liabilities> GetLiabilityListById(int id)
        {
            Entities entities = new Entities();
            Liabilities liability = GetLiabilityById(id);
            List<Liabilities> result;
            if (!liability.ParentLiabilityId.HasValue)
            {
                result = entities.Liabilities.Where(x => (x.Id == id || x.ParentLiabilityId == id) && !x.DisabledDate.HasValue).ToList();
            }
            else
            {
                result = entities.Liabilities.Where(x => (x.Id == liability.ParentLiabilityId || x.ParentLiabilityId == liability.ParentLiabilityId) && !x.DisabledDate.HasValue).ToList();
            }

            return result;
        }

        public static int CreateLiability(Liabilities liability, string username)
        {
            Entities entities = new Entities();
            liability.CreatedDate = DateTime.Now;
            liability.CreatedBy = Constants.Constants.USER;
            liability.Username = username;
            entities.Liabilities.Add(liability);

            if (liability.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE ||
               liability.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS)
            {
                Liabilities childLiability = new Liabilities();
                childLiability.Name = liability.Name;
                childLiability.Value = liability.Value;
                childLiability.InterestRate = liability.InterestRate;
                childLiability.StartDate = liability.StartDate;
                childLiability.EndDate = liability.EndDate;
                childLiability.LiabilityType = liability.LiabilityType;
                childLiability.CreatedDate = liability.CreatedDate;
                childLiability.CreatedBy = Constants.Constants.USER;
                childLiability.AssetId = liability.AssetId;
                childLiability.Liabilities1.Add(liability);
                childLiability.Username = username;
                childLiability.InterestType = liability.InterestType;
                entities.Liabilities.Add(childLiability);
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static double GetCurrentInterestRate(int parentLoanId)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            Entities entities = new Entities();
            double interestRate = entities.Liabilities.Where(x => x.ParentLiabilityId.HasValue && x.ParentLiabilityId == parentLoanId && !x.DisabledDate.HasValue && x.StartDate <= current && x.EndDate > current).FirstOrDefault().InterestRate;
            return interestRate;
        }

        public static AssetListViewModel GetLiabilityListViewModelByAssetListViewModel(AssetListViewModel assetListViewModel)
        {
            Entities entities = new Entities();
            AssetListViewModel result = new AssetListViewModel();
            result.Type = assetListViewModel.Type;
            foreach (var assetViewModel in assetListViewModel.List)
            {
                assetViewModel.LiabilityList = GetLiabilityViewModelByAssetViewModel(assetViewModel);
                result.List.Add(assetViewModel);
            }
            return result;
        }

        public static LiabilityListViewModel GetLiabilityViewModelByAssetViewModel(AssetViewModel assetViewModel)
        {
            Entities entities = new Entities();
            LiabilityListViewModel result = new LiabilityListViewModel();
            IQueryable<Liabilities> liabilities = entities.Liabilities.Where(x => x.AssetId == assetViewModel.Asset.Id && !x.DisabledDate.HasValue);
            foreach (var liability in liabilities)
            {
                IQueryable<Liabilities> list;
                if (liability.ParentLiabilityId.HasValue)
                {
                    list = liabilities.Where(x => x.Id == liability.ParentLiabilityId || x.ParentLiabilityId == liability.ParentLiabilityId);
                }
                else
                {
                    list = liabilities.Where(x => x.Id == liability.Id || x.ParentLiabilityId == liability.Id);
                }

                LiabilityViewModel liabilityViewModel = new LiabilityViewModel();
                DateTime current = DateTime.Now;
                current = new DateTime(current.Year, current.Month, 1);

                Liabilities parentLiability = list.Where(x => !x.ParentLiabilityId.HasValue).FirstOrDefault();

                liabilityViewModel.Liability = liability;
                liabilityViewModel.TotalPaymentPeriod = FormatUtility.CalculateTimePeriod(liability.StartDate, liability.EndDate) + 1;
                if (liability.ParentLiabilityId.HasValue)
                {
                    liabilityViewModel.CurrentInterestRate = liability.InterestRate;
                }
                else
                {
                    liabilityViewModel.CurrentInterestRate = GetCurrentInterestRate(parentLiability.Id);
                }
                if (liability.StartDate <= current && current <= liability.EndDate)
                {
                    liabilityViewModel.MonthlyOriginalPayment = liability.Value / FormatUtility.CalculateTimePeriod(parentLiability.StartDate, parentLiability.EndDate);

                    int currentPeriod = FormatUtility.CalculateTimePeriod(parentLiability.StartDate, DateTime.Now);

                    if (currentPeriod > 0)
                    {
                        liabilityViewModel.RemainedValue = liability.Value - currentPeriod * liabilityViewModel.MonthlyOriginalPayment;
                        liabilityViewModel.MonthlyInterestPayment = liabilityViewModel.RemainedValue * liabilityViewModel.CurrentInterestRate / 1200;
                    }
                    else
                    {
                        liabilityViewModel.RemainedValue = liability.Value;
                        liabilityViewModel.MonthlyInterestPayment = 0;
                    }

                    liabilityViewModel.MonthlyPayment = liabilityViewModel.MonthlyInterestPayment + liabilityViewModel.MonthlyOriginalPayment;
                    liabilityViewModel.AnnualPayment = liability.Value * liabilityViewModel.CurrentInterestRate / 100;           //chua xu ly// 
                }
                else
                {
                    liabilityViewModel.MonthlyInterestPayment = 0;
                    liabilityViewModel.MonthlyOriginalPayment = 0;
                    liabilityViewModel.MonthlyPayment = 0;
                    liabilityViewModel.AnnualPayment = 0;
                    liabilityViewModel.RemainedValue = 0;
                }

                assetViewModel.LiabilityList.List.Add(liabilityViewModel);
                if (!liability.ParentLiabilityId.HasValue)
                {
                    assetViewModel.TotalMortgageValue += liabilityViewModel.Liability.Value;
                    assetViewModel.TotalInterestPayment += liabilityViewModel.MonthlyInterestPayment;
                    assetViewModel.TotalOriginalPayment += liabilityViewModel.MonthlyOriginalPayment;
                    assetViewModel.TotalMonthlyPayment += liabilityViewModel.MonthlyPayment;
                    assetViewModel.TotalRemainingValue += liabilityViewModel.RemainedValue;
                    assetViewModel.TotalAnnualPayment += liabilityViewModel.AnnualPayment;
                }
                assetViewModel.AverageInterestRate = 100 * assetViewModel.TotalAnnualPayment / assetViewModel.TotalMortgageValue;
            }
            return result;
        }

        public static LiabilityViewModel GetLiabilityViewModelByLiability(Liabilities liability)
        {
            LiabilityViewModel result = new LiabilityViewModel();
            result.Liability = liability;
            int totalPeriod = FormatUtility.CalculateTimePeriod(liability.StartDate, liability.EndDate);
            DateTime current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int currentPeriod = FormatUtility.CalculateTimePeriod(liability.StartDate, current) + 1;

            double interestPerMonth = liability.InterestRate / 1200;
            result.TotalPaymentPeriod = totalPeriod;
            result.MonthlyOriginalPayment = liability.Value / totalPeriod;
            result.MonthlyInterestPayment = liability.Value * interestPerMonth;
            result.MonthlyPayment = result.MonthlyInterestPayment + result.MonthlyInterestPayment;
            result.CurrentInterestRate = liability.InterestRate;
            result.RemainedValue = liability.Value - currentPeriod * result.MonthlyOriginalPayment;

            return result;
        }

        public static List<LiabilityPaymentViewModel> CalculatePaymentsByMonth(List<Liabilities> list, Liabilities liability, bool append)
        {
            List<LiabilityPaymentViewModel> result = new List<LiabilityPaymentViewModel>();

            if (liability.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.REAL_ESTATE ||
                liability.LiabilityType == (int)Constants.Constants.LIABILITY_TYPE.BUSINESS)
            {
                list = list.OrderBy(x => x.StartDate).ToList();

                Liabilities parentLiability = list.Where(x => !x.ParentLiabilityId.HasValue).FirstOrDefault();
                double remainLoan = parentLiability.Value;
                double numberOfOriginalMonths = (parentLiability.EndDate.Year - parentLiability.StartDate.Year) * 12 + parentLiability.EndDate.Month - parentLiability.StartDate.Month;
                double monthlyOriginalPayment = parentLiability.Value / numberOfOriginalMonths;
                DateTime startDate = list.Where(x => x.ParentLiabilityId.HasValue).OrderBy(x => x.StartDate).FirstOrDefault().StartDate;
                double monthlyTotalOriginalPayment = 0;
                foreach (var item in list)
                {
                    if (item.ParentLiabilityId.HasValue)
                    {
                        double interestPerMonth = item.InterestRate / 1200;

                        double numberOfRealMonths = (item.EndDate.Year - item.StartDate.Year) * 12 + item.EndDate.Month - item.StartDate.Month + 1;
                        for (int j = 1; j <= numberOfRealMonths; j++)
                        {
                            LiabilityPaymentViewModel model = new LiabilityPaymentViewModel();
                            if (append)
                            {
                                if (liability.StartDate <= startDate && startDate <= liability.EndDate)
                                {
                                    model.MonthlyOriginalPayment = monthlyOriginalPayment;
                                    model.MonthlyInterestPayment = remainLoan * liability.InterestRate / 1200;
                                    model.CurrentInterestRatePerYear = liability.InterestRate;
                                    model.CurrentMonth = startDate;
                                    model.Highlight = true;
                                }
                                else
                                {
                                    model.MonthlyOriginalPayment = monthlyOriginalPayment;
                                    model.MonthlyInterestPayment = remainLoan * interestPerMonth;
                                    model.CurrentInterestRatePerYear = item.InterestRate;
                                    model.CurrentMonth = startDate;
                                }
                            }
                            else
                            {
                                model.MonthlyOriginalPayment = monthlyOriginalPayment;
                                model.MonthlyInterestPayment = remainLoan * interestPerMonth;
                                model.CurrentInterestRatePerYear = item.InterestRate;
                                model.CurrentMonth = startDate;
                            }

                            model.MonthlyTotalPayment = model.MonthlyOriginalPayment + model.MonthlyInterestPayment;
                            model.RemainingLoan = remainLoan;

                            if (!append)
                            {
                                if (liability.StartDate <= startDate && startDate <= liability.EndDate)
                                {
                                    result.Add(model);
                                }
                            }
                            else
                            {
                                result.Add(model);
                            }

                            monthlyTotalOriginalPayment += monthlyOriginalPayment;
                            remainLoan -= monthlyOriginalPayment;
                            startDate = startDate.AddMonths(1);
                        }
                    }
                }
            }
            else
            {
                int totalPeriod = FormatUtility.CalculateTimePeriod(liability.StartDate, liability.EndDate);
                DateTime currentMonth = liability.StartDate;
                for (int i = 0; i < totalPeriod; i++)
                {
                    LiabilityPaymentViewModel model = new LiabilityPaymentViewModel
                    {
                        CurrentInterestRatePerYear = liability.InterestRate,
                        CurrentMonth = currentMonth,
                        Highlight = false,
                        MonthlyInterestPayment = liability.Value * liability.InterestRate / 1200,
                        MonthlyOriginalPayment = liability.Value / totalPeriod
                    };
                    model.MonthlyTotalPayment = model.MonthlyInterestPayment + model.MonthlyOriginalPayment;
                    model.RemainingLoan = liability.Value - (i + 1) * model.MonthlyOriginalPayment;
                    currentMonth = currentMonth.AddMonths(1);
                    result.Add(model);
                }
            }
            return result;
        }

        public static int UpdateLiability(Liabilities data)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;
            int result = 0;

            Liabilities parentLiability = entities.Liabilities.Where(x => x.Id == data.Id).FirstOrDefault();
            List<Liabilities> liabilities = entities.Liabilities.Where(x => x.ParentLiabilityId == data.Id && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate).ToList();

            if (data.InterestRate > 0)
            {
                Liabilities start = liabilities.Where(x => x.StartDate < data.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault();
                if (start != null)
                {
                    start.EndDate = data.StartDate.AddMonths(-1);
                    entities.Liabilities.Attach(start);
                    var entry = entities.Entry(start);
                    entry.Property(x => x.EndDate).IsModified = true;
                }

                List<Liabilities> middle = liabilities.Where(x => x.StartDate >= data.StartDate && x.EndDate <= data.EndDate).ToList();
                if (middle.Any())
                {
                    foreach (var item in middle)
                    {
                        item.DisabledDate = current;
                        item.DisabledBy = Constants.Constants.USER;
                    }
                }

                Liabilities updated_liability = new Liabilities();
                updated_liability.Name = parentLiability.Name;
                updated_liability.Value = parentLiability.Value;
                updated_liability.InterestType = parentLiability.InterestType;
                updated_liability.InterestRate = data.InterestRate;
                updated_liability.CreatedDate = current;
                updated_liability.CreatedBy = Constants.Constants.USER;
                updated_liability.StartDate = data.StartDate;
                updated_liability.EndDate = data.EndDate;
                updated_liability.AssetId = parentLiability.AssetId;
                updated_liability.ParentLiabilityId = parentLiability.Id;
                updated_liability.LiabilityType = parentLiability.LiabilityType;
                entities.Liabilities.Add(updated_liability);

                Liabilities end = liabilities.Where(x => x.EndDate > data.EndDate).OrderBy(x => x.EndDate).FirstOrDefault();
                if (end != null)
                {
                    end.StartDate = data.EndDate.AddMonths(1);
                    entities.Liabilities.Attach(end);
                    var entry = entities.Entry(end);
                    entry.Property(x => x.StartDate).IsModified = true;
                    entry.Property(x => x.InterestRate).IsModified = true;
                }
                else if (data.EndDate != parentLiability.EndDate)
                {
                    end = new Liabilities();
                    end.Name = parentLiability.Name;
                    end.Value = parentLiability.Value;
                    end.InterestType = parentLiability.InterestType;
                    end.InterestRate = parentLiability.InterestRate;
                    end.CreatedDate = current;
                    end.CreatedBy = Constants.Constants.USER;
                    end.StartDate = data.EndDate.AddMonths(1);
                    end.EndDate = parentLiability.EndDate;
                    end.AssetId = parentLiability.AssetId;
                    end.ParentLiabilityId = parentLiability.Id;
                    end.LiabilityType = parentLiability.LiabilityType;
                    entities.Liabilities.Add(end);
                }

                result = entities.SaveChanges();
            }
            else
            {
                parentLiability.Name = data.Name;
                parentLiability.Value = data.Value;
                parentLiability.InterestType = data.InterestType;
                parentLiability.StartDate = data.StartDate;
                parentLiability.EndDate = data.EndDate;

                entities.Liabilities.Attach(parentLiability);
                var entry = entities.Entry(parentLiability);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.Value).IsModified = true;
                entry.Property(x => x.InterestType).IsModified = true;
                entry.Property(x => x.StartDate).IsModified = true;
                entry.Property(x => x.EndDate).IsModified = true;

                foreach (var liability in liabilities)
                {
                    liability.DisabledDate = current;
                    liability.DisabledBy = Constants.Constants.USER;
                    entities.Liabilities.Attach(liability);
                    entry = entities.Entry(liability);
                    entry.Property(x => x.DisabledDate).IsModified = true;
                    entry.Property(x => x.DisabledBy).IsModified = true;
                }

                Liabilities childLiability = new Liabilities();
                childLiability.Name = data.Name;
                childLiability.Value = data.Value;
                childLiability.InterestType = data.InterestType;
                childLiability.InterestRate = parentLiability.InterestRate;
                childLiability.StartDate = data.StartDate;
                childLiability.EndDate = data.EndDate;
                childLiability.CreatedDate = current;
                childLiability.CreatedBy = Constants.Constants.USER;
                childLiability.AssetId = parentLiability.AssetId;
                childLiability.ParentLiabilityId = data.Id;
                childLiability.LiabilityType = parentLiability.LiabilityType;

                entities.Liabilities.Add(childLiability);
                result = entities.SaveChanges();
            }
            return result;
        }

        public static int DeleteLiability(int id)
        {
            Entities entities = new Entities();
            List<Liabilities> liabilities = entities.Liabilities.Where(x => (x.Id == id || x.ParentLiabilityId == id) && !x.DisabledDate.HasValue).ToList();
            foreach (var liability in liabilities)
            {
                liability.DisabledDate = DateTime.Now;
                liability.DisabledBy = Constants.Constants.USER;
                entities.Liabilities.Attach(liability);
                var entry = entities.Entry(liability);
                entry.Property(x => x.DisabledDate).IsModified = true;
                entry.Property(x => x.DisabledBy).IsModified = true;
            }
            int result = entities.SaveChanges();
            return result;
        }
    }
}