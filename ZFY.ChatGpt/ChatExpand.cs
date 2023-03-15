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
                config.DefaultRequestHeaders.Add("Accept", "application/json");
                config.DefaultRequestHeaders.Add("Authorization", $"Bearer {Constants.ApiKey}");
                //config.DefaultRequestHeaders.Add("header_1", "header_1");
            });//.AddHttpMessageHandler<ChatHttpHandler>();
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