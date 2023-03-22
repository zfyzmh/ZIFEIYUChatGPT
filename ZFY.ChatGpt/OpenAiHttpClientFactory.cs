using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace ZFY.ChatGpt
{
    /// <summary>
    ///
    /// </summary>
    public class OpenAiHttpClientFactory : IHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public OpenAiHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HttpClient CreateClient(string name)
        {
            var client = _httpClientFactory.CreateClient(name);
            client.Timeout = TimeSpan.FromSeconds(ChatServiceProvider.ServiceProvider.GetService<ChatOption>()!.Timeout);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ChatServiceProvider.ServiceProvider.GetService<ChatOption>()!.ApiKey}");
            return client;
        }
    }
}