using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ZFY.ChatGpt.Services;

namespace ZFY.ChatGpt
{
    /// <summary>
    ///
    /// </summary>
    public static class ChatExpand
    {
        public static IServiceCollection AddChatGPT(this IServiceCollection services)
        {
            return AddChatGPT(services, null);
        }

        public static IServiceCollection AddChatGPT(this IServiceCollection services, ChatOption? chatOption)
        {
            chatOption ??= new ChatOption();
            services.AddSingleton(chatOption);
            //var chatHttpHandler = new ChatHttpHandler();
            //services.AddSingleton(chatHttpHandler);

            services.AddHttpClient("ChatGPT", config =>
            {
                config.BaseAddress = new Uri("https://api.openai.com");
            })
            .AddHttpMessageHandler(hander => new ChatHttpHandler())
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (chatOption.IsProxy)
                {
                    handler.Proxy = new WebProxy(chatOption.ProxyAddress == string.Empty ? "127.0.0.1" : chatOption.ProxyAddress, chatOption.ProxyPort);
                    handler.UseProxy = true;
                }
                return handler;
            }
            );
            services.AddSingleton<OpenAiHttpClientFactory>();
            services.AddSingleton<ChatServices>();
            ChatServiceProvider.ServiceProvider = services.BuildServiceProvider();
            return services;
        }

        public static HttpClient CreateClient(this OpenAiHttpClientFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            return factory.CreateClient("ChatGPT");
        }
    }

    public static class ChatServiceProvider
    {
        private static ServiceProvider serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set
            {
                serviceProvider ??= value;
            }
        }
    }
}