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
            client.Timeout = TimeSpan.FromSeconds(Constants.Timeout);
            /*new HttpClientHandler()
            {
                Proxy = new WebProxy(Constants.ProxyAddress == string.Empty ? "127.0.0.1" : Constants.ProxyAddress, Constants.ProxyPort),
                UseProxy = Constants.IsProxy
            }*/
            return client;
        }
    }
}