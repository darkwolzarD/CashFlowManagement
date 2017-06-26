using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class UserQueries
    {
        public static Users CheckLogin(LoginViewModel model)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)).FirstOrDefault();
            return user;
        }

        public static string GetCurrentUsername()
        {
            if((Users)HttpContext.Current.Session["USER"] != null)
            {
                return ((Users)HttpContext.Current.Session["USER"]).Username;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get account by username
        /// </summary>
        /// <param name="username">Username of account</param>
        /// <returns>Account</returns>
        public static Users GetUserByUsername(string username)
        {
            Entities entities = new Entities();
            Users result = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            return result;
        }

        public static Users Register(RegisterViewModel model)
        {
            Entities entities = new Entities();

            //Create user
            Users user = new Users();
            user.Username = model.Username;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Password = model.Password;
            user.Sex = model.Sex;
            user.Job = model.Job;
            user.NumberOfChildren = model.NumberOfChildren;
            user.IncomeInitialized = false;
            user.RealEstateInitialized = false;
            user.BusinessInitialized = false;
            user.BankDepositInitialized = false;
            user.StockInitialized = false;
            user.InsuranceInitialized = false;
            user.OtherAssetInitialized = false;
            user.CarLiabilityInitialized = false;
            user.CreditCardInitialized = false;
            user.OtherLiabilityInitialized = false;
            user.FamilyExpenseInitialized = false;
            user.OtherExpenseInitialized = false;
            user.CreatedDate = DateTime.Now;

            //Add user
            entities.Users.Add(user);
            entities.SaveChanges();
            return user;
        }

        public static Users IncomeInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.IncomeInitialized)
            {
                user.IncomeInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                int result = entities.SaveChanges();
            }
            return user;
        }
        public static Users OtherAssetInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherAssetInitialized)
            {
                user.OtherAssetInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                int result = entities.SaveChanges();
            }
            return user;
        }

        public static Users OtherLiabilityInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherLiabilityInitialized)
            {
                user.OtherLiabilityInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                int result = entities.SaveChanges();
            }
            return user;
        }

        public static Users OtherExpenseInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherExpenseInitialized)
            {
                user.OtherExpenseInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                int result = entities.SaveChanges();
            }
            return user;
        }
    }
}