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
            services.AddSingleton<ChatServices>();
            services.AddHttpClient("ChatGPT", config =>
            {
                config.BaseAddress = new Uri("https://api.openai.com");
                //config.DefaultRequestHeaders.Add("header_1", "header_1");
            }).AddHttpMessageHandler<ChatHttpHandler>()

            ;
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