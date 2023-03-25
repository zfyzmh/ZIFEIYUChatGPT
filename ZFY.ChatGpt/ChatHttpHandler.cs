using Microsoft.Extensions.DependencyInjection;

namespace ZFY.ChatGpt
{
    /// <summary>
    /// 自定义Handler用于添加请求头及配置项
    /// </summary>
    public class ChatHttpHandler : DelegatingHandler
    {
        private CancellationTokenSource? tokenSource;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            tokenSource ??= new CancellationTokenSource();

            return await base.SendAsync(request, tokenSource.Token);
        }

        public async Task Cancel()
        {
            tokenSource ??= new CancellationTokenSource();
            tokenSource.Cancel(); tokenSource.Dispose(); tokenSource = null;
        }
    }
}