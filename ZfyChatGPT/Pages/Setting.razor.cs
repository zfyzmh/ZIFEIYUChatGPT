using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics;
using ZfyChatGPT.Entity;
using ZfyChatGPT.Global;
using ZfyChatGPT.Services;

namespace ZfyChatGPT.Pages
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

        public string ProxyAddress
        {
            get { return UserConfig.ProxyAddress; }
            set
            {
                UserConfig.ProxyAddress = value;
                UserServices!.UpdateConfig(UserConfig);
                if (Proxy_Switch) _ = RestartApp();
            }
        }

        public int ProxyPort
        {
            get { return UserConfig.ProxyPort; }
            set
            {
                UserConfig.ProxyPort = value;
                UserServices!.UpdateConfig(UserConfig);
                if (Proxy_Switch) _ = RestartApp();
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
                RestartApp();
            }
        }

        [Inject] private IDialogService? DialogService { get; set; }

        /// <summary>
        /// 重启应用
        /// </summary>
        public async Task RestartApp()
        {
            bool? result = await DialogService!.ShowMessageBox(
            "提示!",
            "更改此配置需重启应用以响应更新!",
            yesText: "立刻重启!", cancelText: "稍后手动重启");

            if (result.Value)
            {
                var startInfo = new ProcessStartInfo(
                Process.GetCurrentProcess().MainModule.FileName);
                Process.Start(startInfo);
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}