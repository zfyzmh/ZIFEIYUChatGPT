﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }
    }
}