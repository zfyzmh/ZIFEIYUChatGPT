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
            services.AddHttpClient("ChatGPT", config =>
            {
                config.BaseAddress = new Uri("https://api.openai.com");
            })
            .AddHttpMessageHandler(hander => new ChatHttpHandler())
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    Proxy = new WebProxy(Constants.ProxyAddress == string.Empty ? "127.0.0.1" : Constants.ProxyAddress, Constants.ProxyPort),
                    UseProxy = Constants.IsProxy
                };
            }

            );
            services.AddSingleton<OpenAiHttpClientFactory>();
            services.AddSingleton<ChatServices>();
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
}