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
            Users user = entities.Users.Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password)).FirstOrDefault();
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
            user.Username = model.Email;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
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
            user.AvailableMoneyInitialized = false;
            user.CreatedDate = DateTime.Now;

            //Add user
            entities.Users.Add(user);
            entities.SaveChanges();
            return user;
        }

        public static bool CheckUniqueUser(string username)
        {
            Entities entities = new Entities();
            return entities.Users.Where(x => x.Username.Equals(username)).Any();
        }

        public static bool CheckUniquePhoneNumber(string phoneNumber)
        {
            Entities entities = new Entities();
            return entities.Users.Where(x => x.PhoneNumber.Equals(phoneNumber)).Any();
        }

        public static int SalaryInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.IncomeInitialized)
            {
                user.IncomeInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int RealEstateInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.RealEstateInitialized)
            {
                user.RealEstateInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int BusinessInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.BusinessInitialized)
            {
                user.BusinessInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int BankDepositInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.BankDepositInitialized)
            {
                user.BankDepositInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int StockInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.StockInitialized)
            {
                user.StockInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int InsuranceInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.InsuranceInitialized)
            {
                user.InsuranceInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int OtherAssetInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherAssetInitialized)
            {
                user.OtherAssetInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int CarLiabilityInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.CarLiabilityInitialized)
            {
                user.CarLiabilityInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int CreditCardLiabilityInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.CreditCardInitialized)
            {
                user.CreditCardInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int OtherLiabilityInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherLiabilityInitialized)
            {
                user.OtherLiabilityInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int FamilyExpenseInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.FamilyExpenseInitialized)
            {
                user.FamilyExpenseInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int OtherExpenseInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.OtherExpenseInitialized)
            {
                user.OtherExpenseInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static int AvailableMoneyInitialize(string username)
        {
            Entities entities = new Entities();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            if (!user.AvailableMoneyInitialized)
            {
                user.AvailableMoneyInitialized = true;
                entities.Users.Attach(user);
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                return entities.SaveChanges();
            }
            return -1;
        }

        public static bool IsSalaryInitialized(string username)
        {
            Entities entities = new Entities();
            return entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault().IncomeInitialized;
        }
    }
}