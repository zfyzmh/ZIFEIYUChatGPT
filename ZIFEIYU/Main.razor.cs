using MudBlazor;
using ZIFEIYU.Global;
using ZIFEIYU.Services;

namespace ZIFEIYU
{
    public partial class Main : IDisposable
    {
        private MudThemeProvider? _mudThemeProvider;
        private MudTheme _theme = new();
        private bool _isDarkMode = false;

        protected override void OnInitialized()
        {
            EventDispatcher.AddAction("SwitchTheme", (value) =>
            {
                _isDarkMode = !_isDarkMode;
                StateHasChanged();
            });
            EventDispatcher.AddFunc("IsDarkTheme", () =>
            {
                return _isDarkMode;
            });
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var userServices = Common.ServiceProvider.GetService<UserServices>();
            _isDarkMode = (await userServices.GetConfig()).IsDarkMode;
            await userServices.UpdateConfig(await userServices.GetConfig());
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            EventDispatcher.RemoveAction("SwitchTheme");
            EventDispatcher.RemoveFunc("IsDarkTheme");
        }
    }
}