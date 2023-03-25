using Microsoft.Extensions.DependencyInjection;

namespace ZFY.ChatGpt
{
    /// <summary>
    /// 自定义Handler用于添加请求头及配置项
    /// </summary>
    public class ChatHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}