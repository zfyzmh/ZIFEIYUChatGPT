using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        /// 发送sse请求,即逐字回复版,但有可能连接成功但回复时间超长,不建议使用
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

                string code = "";
                bool isCode = false;
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
                                        if (dia.Choices[0].Delta.Content.Contains('`'))
                                        {
                                            isCode = true;
                                            code += content;

                                            if (Regex.Matches(code, "`").Count == 6)
                                            {
                                                diastr += code;
                                                code = "";
                                                message.Content = diastr;
                                                eventHandler.Invoke(this, chatInput.Messages);
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        if (isCode) { code += content; continue; };
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
                chatMsg.Content = "网络连接失败,请检查网络!";
                eventHandler.Invoke(this, chatInput.Messages);
                return;
            }
        }

        public async Task<OutChat> SendChat(InChat chatInput)
        {
            try
            {
                chatInput.Stream = false;
                HttpClient client = _httpClientFactory.CreateClient();
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(chatInput), Encoding.UTF8))
                {
                    HttpResponseMessage response = await client.PostAsync("/v1/chat/completions", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return (await response.Content.ReadAsStringAsync()).ToEntity<OutChat>();
                    }
                    else
                    {
                        return new OutChat();
                    }
                }
            }
            catch (Exception)
            {
                return new OutChat();
            }
        }
    }
}