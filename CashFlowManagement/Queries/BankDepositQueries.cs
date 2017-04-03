using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class BankDepositQueries
    {
        public static List<BankDepositIncomes> GetBankDepositByUser(string username)
        {
            DateTime current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, 1);
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            List<BankDepositIncomes> result = entities.BankDepositIncomes.Where(x => x.Username.Equals(username) && !x.EndDate.HasValue).OrderByDescending(x => x.CapitalValue).ToList();
            return result;
        }

        public static int CreateBankDeposit(BankDepositIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            entities.BankDepositIncomes.Add(data);
            int result = entities.SaveChanges();
            return result;
        }

        public static int UpdateBankDeposit(BankDepositIncomes data)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BankDepositIncomes business = entities.BankDepositIncomes.Where(x => x.Id == data.Id).FirstOrDefault();
            DateTime current = DateTime.Now;

            business.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.BankDepositIncomes.Attach(business);
            var entry = entities.Entry(business);
            entry.Property(x => x.EndDate).IsModified = true;

            BankDepositIncomes updated_bankdeposit = new BankDepositIncomes();
            updated_bankdeposit.Source = data.Source;
            updated_bankdeposit.CapitalValue = data.CapitalValue;
            updated_bankdeposit.InterestYield = data.InterestYield;
            updated_bankdeposit.ParticipantBank = data.ParticipantBank;
            updated_bankdeposit.Note = data.Note;
            updated_bankdeposit.StartDate = new DateTime(current.Year, current.Month, 1);
            updated_bankdeposit.Username = business.Username;
            entities.BankDepositIncomes.Add(updated_bankdeposit);

            int result = entities.SaveChanges();
            return result;
        }

        public static int DeleteBankDeposit(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
            DateTime current = DateTime.Now;
            deposit.EndDate = new DateTime(current.Year, current.Month, 1);
            entities.BankDepositIncomes.Attach(deposit);
            var entry = entities.Entry(deposit);
            entry.Property(x => x.EndDate).IsModified = true;
            int result = entities.SaveChanges();
            return result;
        }

        public static BankDepositIncomes GetBankDepositById(int id)
        {
            CashFlowManagementEntities entities = new CashFlowManagementEntities();
            BankDepositIncomes deposit = entities.BankDepositIncomes.Where(x => x.Id == id).FirstOrDefault();
            return deposit;
        }
    }
}