using System.Net;
using System.Net.Http;
using System.Text;
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

                //client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(chatInput), Encoding.UTF8))
                {
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    CancellationTokenSource CancellationToken = new CancellationTokenSource(30000);

                    HttpResponseMessage response = await client.PostAsync("/v1/chat/completions", httpContent, CancellationToken.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        bool close = true;
                        using (Stream Stream = response.Content.ReadAsStream())
                        {
                            StreamReader streamReader = new StreamReader(Stream);
                            while (close)
                            {
                                var str = streamReader.ReadLine();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    str = str.Remove(0, str.IndexOf('{'));

                                    var dia = JsonHelper.DeserializeJsonToObject<OutChatSSE>(str);

                                    if (!string.IsNullOrEmpty(dia.Choices[0].Delta.Content))
                                    {
                                        diastr += dia.Choices[0].Delta.Content;
                                        message.Content = diastr;
                                        eventHandler.Invoke(this, chatInput.Messages);
                                        if (speed > 0)
                                        {
                                            await Task.Delay(speed--);
                                        }
                                    }
                                    if (dia.Choices[0].FinishReason == "stop")
                                    {
                                        close = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                var chatMsg = chatInput.Messages.LastOrDefault();
                if (string.IsNullOrWhiteSpace(chatMsg.Content))
                {
                    chatMsg .Content= "网络连接超时,请尝试以下方法解决\r\n" +
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
        }

        /*public async Task<OutChat> SendDialogue(InChat chatInput)
        {
            try
            {
                return await HttpHelper.HttpPostAsync<OutChat>("https://api.openai.com/v1/chat/completions", JsonHelper.SerializeObject(chatInput), headers: Headers);
            }
            catch (Exception)
            {
                return new DialogueOutput();
            }
        }*/
    }
}