using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
            }).AddHttpMessageHandler(hander=>  new ChatHttpHandler());
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