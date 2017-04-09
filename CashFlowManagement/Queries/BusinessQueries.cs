using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CashFlowManagement.Queries
{
    public class BusinessQueries
    {
        public static List<BusinessIncomes> GetBusinessByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<BusinessIncomes> result = entities.BusinessIncomes.Where(x => x.Username.Equals(username) && !x.DisabledDate.HasValue)
                                                                        .Select(x => new
                                                                        {
                                                                            x,
                                                                            BusinessLoan = x.BusinessLoan.Where(m => !m.DisabledDate.HasValue)
                                                                        }).AsEnumerable().Select(m => m.x).OrderByDescending(x => x.Income).ToList();
            return result;
        }

        public static int CreateBusiness(BusinessIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.BusinessIncomes.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateBusiness(BusinessIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessIncomes business = entities.BusinessIncomes.Where(x => x.Id == data.Id).Include(x => x.BusinessLoan).FirstOrDefault();
            DateTime current = DateTime.Now;

            BusinessIncomes updated_business = new BusinessIncomes();
            updated_business.Source = data.Source;
            updated_business.Income = data.Income;
            updated_business.CapitalValue = data.CapitalValue;
            updated_business.ParticipantBank = data.ParticipantBank;
            updated_business.Note = data.Note;
            updated_business.StartDate = data.StartDate;
            updated_business.EndDate = data.EndDate;
            updated_business.CreateDate = current;
            updated_business.Username = business.Username;

            foreach (var loan in business.BusinessLoan)
            {
                updated_business.BusinessLoan.Add(new BusinessLoan
                {
                    Source = loan.Source,
                    MortgageValue = loan.MortgageValue,
                    InterestType = loan.InterestType,
                    InterestRatePerYear = loan.InterestRatePerYear,
                    StartDate = loan.StartDate,
                    EndDate = loan.EndDate,
                    CreatedDate = loan.CreatedDate,
                    DisabledDate = loan.DisabledDate,
                    BusinessId = loan.BusinessId,
                    ParentLoanId = loan.ParentLoanId
                });
            }
            entities.BusinessIncomes.Add(updated_business);

            business.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.BusinessIncomes.Attach(business);
            var entry = entities.Entry(business);
            entry.Property(x => x.EndDate).IsModified = true;

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteBusiness(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessIncomes business = entities.BusinessIncomes.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            business.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.BusinessIncomes.Attach(business);
            var entry = entities.Entry(business);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static BusinessIncomes GetBusinessById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessIncomes business = entities.BusinessIncomes.Where(x => x.Id == id).FirstOrDefault();
            return business;
        }

        public static int CreateLoan(BusinessLoan data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            data.CreatedDate = DateTime.Now;
            entities.BusinessLoan.Add(data);
            int result = entities.SaveChanges();

            BusinessLoan childLoan = new BusinessLoan();
            childLoan.Source = data.Source;
            childLoan.MortgageValue = data.MortgageValue;
            childLoan.InterestType = data.InterestType;
            childLoan.InterestRatePerYear = data.InterestRatePerYear;
            childLoan.StartDate = data.StartDate;
            childLoan.EndDate = data.EndDate;
            childLoan.CreatedDate = data.CreatedDate;
            childLoan.BusinessId = data.BusinessId;
            childLoan.ParentLoanId = data.Id;

            entities.BusinessLoan.Add(childLoan);
            result = entities.SaveChanges();
            return result;
        }

        public static int DeleteLoan(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessLoan loan = entities.BusinessLoan.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            loan.DisabledDate = new DateTime(current.Year, current.Month, 1);
            entities.BusinessLoan.Attach(loan);
            var entry = entities.Entry(loan);
            entry.Property(x => x.DisabledDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static BusinessLoan GetLoanById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessLoan loan = entities.BusinessLoan.Where(x => x.Id == id).FirstOrDefault();
            return loan;
        }

        public static List<BusinessLoan> GetLoanByParentId(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BusinessLoan loan = entities.BusinessLoan.Where(x => x.Id == id).FirstOrDefault();
            if (loan.ParentLoanId.HasValue)
            {
                List<BusinessLoan> list = entities.BusinessLoan.Where(x => (x.Id == loan.ParentLoanId || x.ParentLoanId == loan.ParentLoanId) && !x.DisabledDate.HasValue).ToList();
                return list;
            }
            else
            {
                List<BusinessLoan> list = entities.BusinessLoan.Where(x => (x.Id == id || x.ParentLoanId == id) && !x.DisabledDate.HasValue).ToList();
                return list;
            }
        }

        public static int UpdateLoan(BusinessLoan data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            DateTime current = DateTime.Now;
            int result = 0;

            BusinessLoan parentLoan = entities.BusinessLoan.Where(x => x.Id == data.Id).FirstOrDefault();
            List<BusinessLoan> loans = entities.BusinessLoan.Where(x => x.ParentLoanId == data.Id && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate).ToList();

            if (data.InterestRatePerYear > 0)
            {
                BusinessLoan start = loans.Where(x => x.StartDate < data.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault();
                if (start != null)
                {
                    start.EndDate = data.StartDate.AddMonths(-1);
                    entities.BusinessLoan.Attach(start);
                    var entry = entities.Entry(start);
                    entry.Property(x => x.EndDate).IsModified = true;
                }

                List<BusinessLoan> middle = loans.Where(x => x.StartDate >= data.StartDate && x.EndDate <= data.EndDate).ToList();
                if (middle.Any())
                {
                    foreach (var item in middle)
                    {
                        item.DisabledDate = current;
                    }
                }

                BusinessLoan updated_loan = new BusinessLoan();
                updated_loan.Source = parentLoan.Source;
                updated_loan.MortgageValue = parentLoan.MortgageValue;
                updated_loan.InterestType = parentLoan.InterestType;
                updated_loan.InterestRatePerYear = data.InterestRatePerYear;
                updated_loan.CreatedDate = current;
                updated_loan.StartDate = data.StartDate;
                updated_loan.EndDate = data.EndDate;
                updated_loan.BusinessId = parentLoan.BusinessId;
                updated_loan.ParentLoanId = parentLoan.Id;
                entities.BusinessLoan.Add(updated_loan);

                BusinessLoan end = loans.Where(x => x.EndDate > data.EndDate).OrderBy(x => x.EndDate).FirstOrDefault();
                if (end != null)
                {
                    end.StartDate = data.EndDate.AddMonths(1);
                    entities.BusinessLoan.Attach(end);
                    var entry = entities.Entry(end);
                    entry.Property(x => x.StartDate).IsModified = true;
                    entry.Property(x => x.InterestRatePerYear).IsModified = true;
                }
                else if (data.EndDate != parentLoan.EndDate)
                {
                    end = new BusinessLoan();
                    end.Source = parentLoan.Source;
                    end.MortgageValue = parentLoan.MortgageValue;
                    end.InterestType = parentLoan.InterestType;
                    end.InterestRatePerYear = parentLoan.InterestRatePerYear;
                    end.CreatedDate = current;
                    end.StartDate = data.EndDate.AddMonths(1);
                    end.EndDate = parentLoan.EndDate;
                    end.BusinessId = parentLoan.BusinessId;
                    end.ParentLoanId = parentLoan.Id;
                    entities.BusinessLoan.Add(end);
                }

                result = entities.SaveChanges();
            }
            else
            {
                parentLoan.Source = data.Source;
                parentLoan.MortgageValue = data.MortgageValue;
                parentLoan.InterestType = data.InterestType;
                parentLoan.StartDate = data.StartDate;
                parentLoan.EndDate = data.EndDate;

                entities.BusinessLoan.Attach(parentLoan);
                var entry = entities.Entry(parentLoan);
                entry.Property(x => x.Source).IsModified = true;
                entry.Property(x => x.MortgageValue).IsModified = true;
                entry.Property(x => x.InterestType).IsModified = true;
                entry.Property(x => x.StartDate).IsModified = true;
                entry.Property(x => x.EndDate).IsModified = true;

                foreach (var loan in loans)
                {
                    loan.DisabledDate = current;
                    entities.BusinessLoan.Attach(loan);
                    entry = entities.Entry(loan);
                    entry.Property(x => x.DisabledDate).IsModified = true;
                }

                BusinessLoan childLoan = new BusinessLoan();
                childLoan.Source = data.Source;
                childLoan.MortgageValue = data.MortgageValue;
                childLoan.InterestType = data.InterestType;
                childLoan.InterestRatePerYear = parentLoan.InterestRatePerYear;
                childLoan.StartDate = data.StartDate;
                childLoan.EndDate = data.EndDate;
                childLoan.CreatedDate = current;
                childLoan.BusinessId = parentLoan.BusinessId;
                childLoan.ParentLoanId = data.Id;

                entities.BusinessLoan.Add(childLoan);
                result = entities.SaveChanges();
            }
            return result;
        }

        public static double GetCurrentInterestRate(int parentLoanId)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            double interestRate = entities.BusinessLoan.Where(x => x.ParentLoanId.HasValue && x.ParentLoanId == parentLoanId && x.StartDate <= current && x.EndDate > current).FirstOrDefault().InterestRatePerYear;
            return interestRate;
        }
    }
}