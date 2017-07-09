using CashFlowManagement.EntityModel;
using CashFlowManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class AvailableMoneyQueries
    {
        public static int CreateAvailableMoney(AvailableMoneyCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Assets availableMoney = new Assets();
            availableMoney.AssetName = "Tiền mặt có sẵn khởi tạo của " + username;
            availableMoney.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
            availableMoney.CreatedBy = Constants.Constants.USER;
            availableMoney.CreatedDate = current;
            availableMoney.StartDate = current;
            availableMoney.Username = username;
            availableMoney.Value = model.AvailableMoney.Value;

            entities.Assets.Add(availableMoney);
            return entities.SaveChanges();
        }
    }
}