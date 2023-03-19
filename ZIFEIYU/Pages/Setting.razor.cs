using Microsoft.AspNetCore.Components;
using ZIFEIYU.Entity;
using ZIFEIYU.Global;
using ZIFEIYU.Services;

namespace ZIFEIYU.Pages
{
    public partial class Setting
    {
        private UserConfig? userConfig;

        [Inject]
        public UserServices? UserServices { get; set; }

        private UserConfig UserConfig
        {
            get
            {
                userConfig ??= UserServices!.GetConfig().Result;
                return userConfig;
            }

            set => userConfig = value;
        }

        public string ProxyAddress
        {
            get { return UserConfig.ProxyAddress; }
            set
            {
                UserConfig.ProxyAddress = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public string ApiKey
        {
            get { return UserConfig.ApiKey; }
            set
            {
                UserConfig.ApiKey = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public string ChatModel
        {
            get { return UserConfig.ChatModel; }
            set
            {
                UserConfig.ChatModel = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public int TimeOut
        {
            get { return UserConfig.Timeout; }
            set
            {
                UserConfig.Timeout = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public int ProxyPort
        {
            get { return UserConfig.ProxyPort; }
            set
            {
                UserConfig.ProxyPort = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public bool Theme_Switch
        {
            get { return (bool)EventDispatcher.DispatchFunc("IsDarkTheme"); }
            set
            {
                EventDispatcher.DispatchAction("SwitchTheme");
                UserConfig.IsDarkMode = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }

        public bool Proxy_Switch
        {
            get { return UserConfig.IsProxy; }
            set
            {
                UserConfig.IsProxy = value;
                UserServices!.UpdateConfig(UserConfig);
            }
        }
    }
}