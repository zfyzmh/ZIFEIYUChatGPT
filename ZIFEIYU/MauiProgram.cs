using MudBlazor.Services;
using System.Reflection;
using System.Runtime.ExceptionServices;
using ZIFEIYU.Dao;
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
        Common.ServiceProvider = builder.Services.BuildServiceProvider();
        Common.ServiceProvider.GetService<ZFYDatabase>()!.Init();
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
        return builder.Build();

        void ConfigureServices(IServiceCollection services)
        {
            services.AddMauiBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddMudServices();
            services.AddSingleton<ZFYDatabase>();
            services.AddSingleton<ChatDao>();
            //批量注入服务
            var servicesTypes = Assembly.GetExecutingAssembly().GetTypes().Where(m => m.Namespace == "ZIFEIYU.Services" & m.IsClass & m.IsVisible).ToArray();
            foreach (var servicesType in servicesTypes) { services.AddSingleton(servicesType); }
        }
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception exception = (Exception)e.ExceptionObject;
        Common.ServiceProvider.GetService<ZFYDatabase>()!.ErrorLog(exception);
    }

    private static void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        Common.ServiceProvider.GetService<ZFYDatabase>()!.ErrorLog(e.Exception);
    }
}