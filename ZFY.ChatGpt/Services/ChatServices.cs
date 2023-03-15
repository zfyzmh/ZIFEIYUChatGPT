using System.Net.Http;
using System.Text;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;

namespace ZFY.ChatGpt.Services
{
    public class ChatServices
    {
        private readonly OpenAiHttpClientFactory _httpClientFactory;

        public ChatServices(OpenAiHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        public async Task SendSSEChat(InChat chatInput, EventHandler<List<ChatMessage>> eventHandler)
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

                HttpResponseMessage response = await client.PostAsync("/v1/chat/completions", httpContent);

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