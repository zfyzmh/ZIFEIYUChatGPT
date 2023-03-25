using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Threading;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;

namespace ZFY.ChatGpt.Services
{
    /// <summary>
    /// chat对话服务
    /// </summary>
    public class ChatServices
    {
        private readonly OpenAiHttpClientFactory _httpClientFactory;

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public ChatServices(OpenAiHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// openai 开放出来的对话接口如果设置 stream为true 则获取不到该次请求消耗的令牌数,
        /// 是的，这是 OpenAI API 目前的行为。如果在调用 OpenAI API 时将 stream 参数设置为 true，则无法获取该次请求消耗的令牌数。这是因为流式传输的方式不适用于令牌计数。
        /// 如果您需要获取使用的令牌数，请将 stream 参数设置为 false 或不指定该参数。在这种情况下，您将能够访问响应中的 "X-Request-ID" 和 "X-Usage" 标头，这些标头将提供有关使用的令牌数的详细信息。
        ///请注意，在流式传输模式下，由于请求和响应之间的延迟更长，因此可能会使用更多的令牌。因此，如果您需要最大程度地优化令牌使用，请避免使用流式传输模式。发送sse请求,即逐字回复版,在流式传输模式下，由于请求和响应之间的延迟更长，因此可能会使用更多的令牌。因此，如果您需要最大程度地优化令牌使用，请避免使用流式传输模式。
        /// </summary>
        /// <param name="chatInput"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public async Task SendSSEChat(InChat chatInput, EventHandler<List<ChatMessage>> eventHandler)
        {
            try
            {
                int speed = 100;
                string diastr = "";
                ChatMessage message = new ChatMessage("assistant");
                chatInput.Messages.Add(message);

                chatInput.Stream = true;
                HttpClient client = _httpClientFactory.CreateClient();

                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(chatInput), Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = await client.PostAsync("/v1/chat/completions", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        bool close = false;
                        using (Stream Stream = response.Content.ReadAsStream())
                        {
                            StreamReader streamReader = new StreamReader(Stream);
                            while (!close)
                            {
                                var str = streamReader.ReadLine();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    str = str.Remove(0, str.IndexOf('{'));

                                    var dia = JsonHelper.DeserializeJsonToObject<OutChatSSE>(str);

                                    if (!string.IsNullOrEmpty(dia.Choices[0].Delta.Content))
                                    {
                                        var content = dia.Choices[0].Delta.Content;
                                        diastr += content;
                                        message.Content = diastr;
                                        eventHandler.Invoke(this, chatInput.Messages);
                                        if (speed > 0)
                                        {
                                            await Task.Delay(speed--);
                                        }
                                    }
                                    if (dia.Choices[0].FinishReason == "stop")
                                    {
                                        close = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var chatMsg = chatInput.Messages.LastOrDefault();
                        chatMsg.Content = "ApiKey未设置或已过期!";
                        eventHandler.Invoke(this, chatInput.Messages);
                        return;
                    }
                    else
                    {
                        var chatMsg = chatInput.Messages.LastOrDefault();
                        chatMsg.Content = response.Content.ToString();
                        eventHandler.Invoke(this, chatInput.Messages);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                var chatMsg = chatInput.Messages.LastOrDefault();
                if (string.IsNullOrWhiteSpace(chatMsg.Content))
                {
                    chatMsg.Content = "网络连接超时,请尝试以下方法解决\r\n" +
                    "1.检查网络设置\r\n" +
                    "2.设置更大的容忍超时时长\r\n" +
                    "3.在chatgpt访问量更少时使用";
                }
                else
                {
                    chatMsg.Content += "\r\n>>>>>>>>>>>>>>>连接超时中断!";
                }

                eventHandler.Invoke(this, chatInput.Messages);
                return;
            }
            catch (Exception ex)
            {
                var chatMsg = chatInput.Messages.LastOrDefault();
                chatMsg.Content = "网络连接失败,请检查网络!" +
                    "\r\n" + ex.Message + "\r\n";
                ;
                eventHandler.Invoke(this, chatInput.Messages);
                return;
            }
        }

        public async Task<OutChat> SendChat(InChat chatInput)
        {
            chatInput.Stream = false;
            HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(chatInput), Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = await client.PostAsync("/v1/chat/completions", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return (await response.Content.ReadAsStringAsync()).ToEntity<OutChat>();
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var msg = new ChatMessage() { Role = "assistant", Content = "ApiKey未设置或已过期" };
                        return new OutChat() { Choices = new Choice[] { new Choice() { Message = msg } } };
                    }
                    else
                    {
                        var msg = new ChatMessage() { Role = "assistant", Content = "访问chatgpt失败,状态码:" + response.StatusCode.ToString() };
                        return new OutChat() { Choices = new Choice[] { new Choice() { Message = msg } } };
                    }
                }
            }
            catch (TaskCanceledException)
            {
                string errprMessage = "网络连接超时,或请求已取消,请尝试以下方法解决\r\n" +
                    "1.检查网络设置\r\n" +
                    "2.设置更大的容忍超时时长\r\n" +
                    "3.在chatgpt访问量更少时使用";

                var msg = new ChatMessage() { Role = "assistant", Content = errprMessage };

                return new OutChat() { Choices = new Choice[] { new Choice() { Message = msg } } };
            }
            catch (Exception ex)
            {
                string errprMessage = "网络连接失败,请检查网络配置!" +
                    "\r\n" + ex.Message + "\r\n";
                ;
                var msg = new ChatMessage() { Role = "assistant", Content = errprMessage };
                return new OutChat() { Choices = new Choice[] { new Choice() { Message = msg } } };
            }
        }
    }
}