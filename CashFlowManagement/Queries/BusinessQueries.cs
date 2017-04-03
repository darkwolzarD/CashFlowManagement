using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class BusinessQueries
    {
        public static List<BusinessIncomes> GetBusinessByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<BusinessIncomes> result = entities.BusinessIncomes.Where(x => x.Username.Equals(username) && !x.EndDate.HasValue).OrderByDescending(x => x.Income).ToList();
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
            BusinessIncomes business = entities.BusinessIncomes.Where(x => x.Id == data.Id).FirstOrDefault();
            DateTime current = DateTime.Now;

            business.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.BusinessIncomes.Attach(business);
            var entry = entities.Entry(business);
            entry.Property(x => x.EndDate).IsModified = true;

            BusinessIncomes updated_business = new BusinessIncomes();
            updated_business.Source = data.Source;
            updated_business.Income = data.Income;
            updated_business.CapitalValue = data.CapitalValue;
            updated_business.LoanValue = data.LoanValue;
            updated_business.ExpenseInterest = data.ExpenseInterest;
            updated_business.ParticipantBank = data.ParticipantBank;
            updated_business.Note = data.Note;
            updated_business.StartDate = new DateTime(current.Year, current.Month, 1);
            updated_business.Username = business.Username;
            entities.BusinessIncomes.Add(updated_business);

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
    }
}