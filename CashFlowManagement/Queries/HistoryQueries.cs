using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class HistoryQueries
    {
        public static List<History> GetHistoryByUser(string username)
        {
            Entities entities = new Entities();
            return entities.History.Where(x => x.Username.Equals(username) && x.CreatedBy.Equals(Constants.Constants.USER)).ToList();
        }
    }
}