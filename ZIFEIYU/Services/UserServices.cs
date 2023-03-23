using MudBlazor;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;

namespace ZIFEIYU.Services
{
    public class UserServices
    {
        private readonly ZFYDatabase database;
        private readonly ZFY.ChatGpt.ChatOption chatOption;

        public UserServices(ZFYDatabase database, ZFY.ChatGpt.ChatOption chatOption)
        {
            this.database = database;
            this.chatOption = chatOption;
        }

        public Task UpdateConfig(UserConfig userConfig)
        {
            chatOption.ChatModel = userConfig.ChatModel;
            chatOption.Timeout = userConfig.Timeout;
            chatOption.IsProxy = userConfig.IsProxy;
            chatOption.ProxyAddress = userConfig.ProxyAddress;
            chatOption.ProxyPort = userConfig.ProxyPort;
            chatOption.ApiKey = userConfig.ApiKey;
            return database.Database.UpdateAsync(userConfig);
        }

        public Task<UserConfig> GetConfig()
        {
            return database.Database.Table<UserConfig>().FirstAsync();
        }
    }
}