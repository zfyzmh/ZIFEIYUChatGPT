using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using MudBlazor.Services;
using System.Reflection;
using ZFY.ChatGpt;
using System.Runtime.ExceptionServices;
using ZfyChatGPT.Global;
using ZfyChatGPT.Dao;

namespace ZfyChatGPT;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;

        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        })
        .UseMauiCommunityToolkit();

        ConfigureServices(builder.Services);

        Common.ServiceProvider = builder.Services.BuildServiceProvider();
        Common.ServiceProvider.GetService<ZFYDatabase>()!.Init();
        return builder.Build();

        void ConfigureServices(IServiceCollection services)
        {
            services.AddMauiBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            services.AddSingleton<IFileSaver>(FileSaver.Default);
            services.AddMudServices();
            services.AddSingleton<ZFYDatabase>();
            services.AddSingleton<ChatDao>();

            //批量注入服务
            var servicesTypes = Assembly.GetExecutingAssembly().GetTypes().Where(m => m.Namespace == "ZfyChatGPT.Services" & m.IsClass & m.IsVisible).ToArray();

            services.AddChatGPT();

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