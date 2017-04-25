using CashFlowManagement.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowManagement.Queries
{
    public class LogQueries
    {
        public static List<Log> GetLogByUser(string username, string createdBy)
        {
            Entities entities = new Entities();
            var result = entities.Log.Where(x => x.Username.Equals(username));
            if (!string.IsNullOrEmpty(createdBy))
            {
                result = result.Where(x => x.CreatedBy.Equals(createdBy));
            }
            return result.OrderBy(x => x.Date).ToList();
        }

        public static int CreateLog(Log log, string username)
        {
            int result = 0;
            Entities entities = new Entities();
            log.CreatedDate = DateTime.Now;
            log.CreatedBy = Constants.Constants.USER;
            log.Username = username;
            double currentAvailableMoney = entities.Assets.Where(x => x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY &&
                                                                      !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();
            if (log.LogType == (int)Constants.Constants.LOG_TYPE.INCOME)
            {
                log.AvailableMoney = currentAvailableMoney + log.Value;
            }
            else if (log.LogType == (int)Constants.Constants.LOG_TYPE.EXPENSE)
            {
                log.AvailableMoney = currentAvailableMoney - log.Value;
            }

            Assets availableMoney = new Assets
            {
                AssetName = string.Empty,
                AssetType = (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY,
                CreatedDate = DateTime.Now,
                CreatedBy = Constants.Constants.USER,
                Value = log.LogType == (int)Constants.Constants.LOG_TYPE.INCOME ? log.Value : 0 - log.Value,
                Username = username
            };

            entities.Log.Add(log);
            entities.Assets.Add(availableMoney);

            result = entities.SaveChanges();
            return result;
        }

        public static Log CreateLog(int type, string logContent, string username, double value)
        {
            Entities entities = new Entities();
            double currentAvailableMoney = entities.Assets.Where(x => x.AssetType == (int)Constants.Constants.ASSET_TYPE.AVAILABLE_MONEY
                                                                && !x.DisabledDate.HasValue).Select(x => x.Value).DefaultIfEmpty(0).Sum();

            string content = string.Empty;
            if (type == (int)Constants.Constants.LOG_TYPE.ADD)
            {
                content += "Tạo mới ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.BUY)
            {
                content += "Mua ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.DELETE)
            {
                content += "Xóa ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.EXPENSE)
            {
                content += "Chi ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.INCOME)
            {
                content += "Thu ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.SELL)
            {
                content += "Bán ";
            }
            else if (type == (int)Constants.Constants.LOG_TYPE.UPDATE)
            {
                content += "Cập nhật ";
            }

            content += logContent;
            Log log = new Log
            {
                AvailableMoney = currentAvailableMoney,
                CreatedBy = Constants.Constants.SYSTEM,
                CreatedDate = DateTime.Now,
                Date = DateTime.Now,
                LogContent = content,
                LogType = type,
                Username = username,
                Value = value
            };
            return log;
        }
    }
}