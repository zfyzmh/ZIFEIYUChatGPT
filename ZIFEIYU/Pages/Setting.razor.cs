using Microsoft.AspNetCore.Components;
using ZIFEIYU.Entity;
using ZIFEIYU.Global;
using ZIFEIYU.Services;

namespace ZIFEIYU.Pages
{
    public partial class Setting
    {
        [Inject]
        public UserServices? UserServices { get; set; }

        private UserConfig userConfig { get; set; }

        public bool Theme_Switch
        {
            get { return (bool)EventDispatcher.DispatchFunc("IsDarkTheme"); }
            set
            {
                EventDispatcher.DispatchAction("SwitchTheme");
                userConfig.IsDarkMode = Theme_Switch ? 0 : 1;
                UserServices!.UpdateConfig(userConfig);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            userConfig = await UserServices!.GetConfig();

            await base.OnInitializedAsync();
        }
    }
}