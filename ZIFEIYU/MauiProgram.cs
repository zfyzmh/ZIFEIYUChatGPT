using MudBlazor.Services;
using System.Net.Http.Headers;
using ZIFEIYU.Data;
using ZIFEIYU.Services;

namespace ZIFEIYU;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        ConfigureServices(builder.Services);

        return builder.Build();

        void ConfigureServices(IServiceCollection services)
        {
            services.AddMauiBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
            services.AddSingleton<WeatherForecastService>();
            //services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1") });
            services.AddSingleton<ChatGPTServices>();
            services.AddMudServices();
        }
    }
}