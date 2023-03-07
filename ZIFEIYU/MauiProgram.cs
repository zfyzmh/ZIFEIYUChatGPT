using MudBlazor.Services;
using ZIFEIYU.DataBase;
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
        builder.Services.BuildServiceProvider().GetService<ZFYDatabase>().Init();
        return builder.Build();

        void ConfigureServices(IServiceCollection services)
        {
            services.AddMauiBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
            //services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1") });
            services.AddSingleton<ChatGPTServices>();
            services.AddMudServices();
            services.AddSingleton<ZFYDatabase>();
        }
    }
}