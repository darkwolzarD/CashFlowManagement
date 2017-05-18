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
            return ((Users)HttpContext.Current.Session["USER"]).Username;
        }
    }
}