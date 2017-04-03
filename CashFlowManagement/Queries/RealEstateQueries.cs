﻿using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CashFlowManagement.Queries
{
    public class RealEstateQueries
    {
        public static List<RealEstateIncomes> GetRealEstateByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<RealEstateIncomes> result = entities.RealEstateIncomes.Where(x => x.Username.Equals(username) && !x.EndDate.HasValue)
                                                                        .Select(x => new
                                                                        {
                                                                            x,
                                                                            Loans = x.Loans.Where(m => m.ParentLoanId == null)
                                                                        }).AsEnumerable().Select(m => m.x).OrderByDescending(x => x.Income).ToList();
            return result;
        }

        public static int CreateRealEstate(RealEstateIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.RealEstateIncomes.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateRealEstate(RealEstateIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            RealEstateIncomes realEstate = entities.RealEstateIncomes.Where(x => x.Id == data.Id).Include(x => x.Loans).FirstOrDefault();
            DateTime current = DateTime.Now;

            RealEstateIncomes updated_realEstate = new RealEstateIncomes();
            updated_realEstate.Source = data.Source;
            updated_realEstate.Income = data.Income;
            updated_realEstate.OriginalValue = data.OriginalValue;
            updated_realEstate.StartDate = new DateTime(current.Year, current.Month, 1);
            updated_realEstate.Username = realEstate.Username;

            foreach (var loan in realEstate.Loans)
            {
                updated_realEstate.Loans.Add(new Loans
                {
                    Source = loan.Source,
                    MortgageValue = loan.MortgageValue,
                    InterestType = loan.InterestType,
                    InterestRatePerYear = loan.InterestRatePerYear,
                    StartDate = loan.StartDate,
                    EndDate = loan.EndDate,
                    OriginalPayment = loan.OriginalPayment,
                    InterestPayment = loan.InterestPayment,
                    CreatedDate = loan.CreatedDate,
                    DisabledDate = loan.DisabledDate
                });
            }
            entities.RealEstateIncomes.Add(updated_realEstate);

            realEstate.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.RealEstateIncomes.Attach(realEstate);
            var entry = entities.Entry(realEstate);
            entry.Property(x => x.EndDate).IsModified = true;

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteRealEstate(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            RealEstateIncomes realEstate = entities.RealEstateIncomes.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            realEstate.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.RealEstateIncomes.Attach(realEstate);
            var entry = entities.Entry(realEstate);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static RealEstateIncomes GetRealEstateById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            RealEstateIncomes realEstate = entities.RealEstateIncomes.Where(x => x.Id == id).FirstOrDefault();
            return realEstate;
        }

        public static int CreateLoan(Loans data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            data.CreatedDate = DateTime.Now;
            entities.Loans.Add(data);
            int result = entities.SaveChanges();

            Loans childLoan = new Loans();
            childLoan.Source = data.Source;
            childLoan.MortgageValue = data.MortgageValue;
            childLoan.InterestType = data.InterestType;
            childLoan.InterestRatePerYear = data.InterestRatePerYear;
            childLoan.StartDate = data.StartDate;
            childLoan.EndDate = data.EndDate;
            childLoan.CreatedDate = data.CreatedDate;
            childLoan.RealEstateIncomeId = data.RealEstateIncomeId;
            childLoan.ParentLoanId = data.Id;

            entities.Loans.Add(childLoan);
            result = entities.SaveChanges();
            return result;
        }

        public static int DeleteLoan(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            Loans loan = entities.Loans.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            loan.DisabledDate = new DateTime(current.Year, current.Month, 1);
            entities.Loans.Attach(loan);
            var entry = entities.Entry(loan);
            entry.Property(x => x.DisabledDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static Loans GetLoanById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            Loans loan = entities.Loans.Where(x => x.Id == id).FirstOrDefault();
            return loan;
        }

        public static List<Loans> GetLoanByParentId(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<Loans> list = entities.Loans.Where(x => x.Id == id || x.ParentLoanId == id).ToList();
            return list;
        }

        public static int UpdateLoan(Loans data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            DateTime current = DateTime.Now;

            Loans parentLoan = entities.Loans.Where(x => x.Id == data.Id).FirstOrDefault();
            List<Loans> loans = entities.Loans.Where(x => x.ParentLoanId == data.Id && !x.DisabledDate.HasValue).OrderByDescending(x => x.StartDate).ToList();


            Loans start = loans.Where(x => x.StartDate <= data.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault();
            if(start != null)
            {
                start.EndDate = data.StartDate;
                entities.Loans.Attach(start);
                var entry = entities.Entry(start);
                entry.Property(x => x.EndDate).IsModified = true;
            }

            List<Loans> middle = loans.Where(x => x.StartDate >= data.StartDate && x.EndDate <= data.EndDate).ToList();
            if (middle.Any())
            {
                foreach (var item in middle)
                {
                    item.DisabledDate = current;
                }
            }

            Loans updated_loan = new Loans();
            updated_loan.Source = data.Source;
            updated_loan.MortgageValue = data.MortgageValue;
            updated_loan.InterestType = data.InterestType;
            updated_loan.InterestRatePerYear = data.InterestRatePerYear;
            updated_loan.CreatedDate = current;
            updated_loan.StartDate = data.StartDate;
            updated_loan.EndDate = data.EndDate;
            updated_loan.OriginalPayment = data.OriginalPayment;
            updated_loan.InterestPayment = data.InterestPayment;
            updated_loan.RealEstateIncomeId = parentLoan.RealEstateIncomeId;
            updated_loan.ParentLoanId = parentLoan.Id;
            entities.Loans.Add(updated_loan);

            Loans end = loans.Where(x => x.EndDate >= data.EndDate).OrderBy(x => x.EndDate).FirstOrDefault();
            if (end != null)
            {
                end.StartDate = data.EndDate;
                if(end.EndDate.Equals(parentLoan.EndDate))
                {
                    end.InterestRatePerYear = data.InterestRatePerYear;
                }
                entities.Loans.Attach(end);
                var entry = entities.Entry(end);
                entry.Property(x => x.StartDate).IsModified = true;
                entry.Property(x => x.InterestRatePerYear).IsModified = true;
            }
            else
            {
                end = new Loans();
                end.Source = data.Source;
                end.MortgageValue = data.MortgageValue;
                end.InterestType = data.InterestType;
                end.InterestRatePerYear = data.InterestRatePerYear;
                end.CreatedDate = current;
                end.StartDate = data.EndDate;
                end.EndDate = parentLoan.EndDate;
                end.OriginalPayment = data.OriginalPayment;
                end.InterestPayment = data.InterestPayment;
                end.RealEstateIncomeId = parentLoan.RealEstateIncomeId;
                end.ParentLoanId = parentLoan.Id;
                entities.Loans.Add(end);
            }

            int result = entities.SaveChanges();
            return result;
        }

        public static double GetCurrentInterestRate(int parentLoanId)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            double interestRate = entities.Loans.Where(x => x.ParentLoanId.HasValue && x.ParentLoanId == parentLoanId && x.StartDate <= current && x.EndDate > current).FirstOrDefault().InterestRatePerYear;
            return interestRate;
        }
    }
}