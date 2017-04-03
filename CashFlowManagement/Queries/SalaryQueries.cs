using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class SalaryQueries
    {
        public static List<Salary> GetSalaryByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<Salary> result = entities.Salary.Where(x => x.Username.Equals(username) && !x.EndDate.HasValue).OrderByDescending(x => x.Income).ToList();
            return result;
        }

        public static int CreateSalary(Salary data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.Salary.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateSalary(Salary data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            Salary salary = entities.Salary.Where(x => x.Id == data.Id).FirstOrDefault();
            DateTime current = DateTime.Now;

            salary.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.Salary.Attach(salary);
            var entry = entities.Entry(salary);
            entry.Property(x => x.EndDate).IsModified = true;

            Salary updated_salary = new Salary();
            updated_salary.Source = salary.Source;
            updated_salary.Income = data.Income;
            updated_salary.StartDate = new DateTime(current.Year, current.Month, 1);
            updated_salary.Username = salary.Username;
            entities.Salary.Add(updated_salary);

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteSalary(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            Salary salary = entities.Salary.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            salary.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.Salary.Attach(salary);
            var entry = entities.Entry(salary);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static Salary GetSalaryById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            Salary salary = entities.Salary.Where(x => x.Id == id).FirstOrDefault();
            return salary;
        }
    }
}