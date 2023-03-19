using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIFEIYU.DataBase;
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
            _isDarkMode = (await Common.ServiceProvider.GetService<UserServices>().GetConfig()).IsDarkMode;

            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            EventDispatcher.RemoveAction("SwitchTheme");
            EventDispatcher.RemoveFunc("IsDarkTheme");
        }
    }
}