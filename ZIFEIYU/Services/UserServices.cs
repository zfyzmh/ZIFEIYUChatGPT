using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using ChatConstants = ZFY.ChatGpt.Constants;

namespace ZIFEIYU.Services
{
    public class UserServices
    {
        private readonly ZFYDatabase database;

        public UserServices(ZFYDatabase database)
        {
            this.database = database;
        }

        public Task UpdateConfig(UserConfig userConfig)
        {
            ChatConstants.ChatModel = userConfig.ChatModel;
            ChatConstants.Timeout = userConfig.Timeout;
            ChatConstants.IsProxy = userConfig.IsProxy;
            ChatConstants.ProxyAddress = userConfig.ProxyAddress;
            ChatConstants.ProxyPort = userConfig.ProxyPort;
            ChatConstants.ApiKey = userConfig.ApiKey;
            return database.Database.UpdateAsync(userConfig);
        }

        public Task<UserConfig> GetConfig()
        {
            return database.Database.Table<UserConfig>().FirstAsync();
        }
    }
}