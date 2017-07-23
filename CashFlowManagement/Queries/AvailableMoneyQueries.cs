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
        public static bool CheckExistAvailableMoney(string username)
        {
            Entities entities = new Entities();
            return entities.Assets.Where(x => x.Username.Equals(username)
                                                          && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                          && !x.DisabledDate.HasValue).Any();
        }
        public static int CreateAvailableMoney(AvailableMoneyCreateViewModel model, string username)
        {
            Entities entities = new Entities();
            DateTime current = DateTime.Now;

            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            Assets availableMoney = entities.Assets.Where(x => x.Username.Equals(username)
                                                          && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                          && !x.DisabledDate.HasValue).OrderBy(x => x.CreatedDate).FirstOrDefault();
            if (availableMoney != null)
            {
                availableMoney.Value = model.AvailableMoney.Value;
                entities.Assets.Attach(availableMoney);
                entities.Entry(availableMoney).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                availableMoney = new Assets();
                availableMoney.AssetName = "Tiền mặt có sẵn khởi tạo của " + user.FullName;
                availableMoney.AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY;
                availableMoney.CreatedBy = Constants.Constants.USER;
                availableMoney.CreatedDate = current;
                availableMoney.StartDate = current;
                availableMoney.Username = username;
                availableMoney.Value = model.AvailableMoney.Value;
                entities.Assets.Add(availableMoney);
            }

            return entities.SaveChanges();
        }

        public static AvailableMoneyCreateViewModel GetInitializedAvailableMoney(string username)
        {
            Entities entities = new Entities();
            AvailableMoneyCreateViewModel viewModel = new AvailableMoneyCreateViewModel();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();

            Assets availableMoney = entities.Assets.Where(x => x.Username.Equals(username)
                                                          && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                          && !x.DisabledDate.HasValue).OrderBy(x => x.CreatedDate).FirstOrDefault();
            if(availableMoney != null)
            {
                viewModel.Name = "Tiền mặt có sẵn khởi tạo của " + user.FullName;
                viewModel.AvailableMoney = availableMoney.Value;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        public static AvailableMoneySummaryViewModel GetInitializedAvailableMoneySummary(string username)
        {
            Entities entities = new Entities();
            AvailableMoneySummaryViewModel viewModel = new AvailableMoneySummaryViewModel();
            Users user = entities.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();

            Assets availableMoney = entities.Assets.Where(x => x.Username.Equals(username)
                                                          && x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                          && !x.DisabledDate.HasValue).OrderBy(x => x.CreatedDate).FirstOrDefault();
            if (availableMoney != null)
            {
                viewModel.Name = "Tiền mặt có sẵn khởi tạo của " + user.FullName;
                viewModel.AvailableMoney = availableMoney.Value;
                return viewModel;
            }
            else
            {
                return null;
            }
        }
    }
}