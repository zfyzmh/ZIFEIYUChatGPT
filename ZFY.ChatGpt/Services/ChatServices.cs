using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;

namespace ZFY.ChatGpt.Services
{
    public class ChatServices
    {
        public async Task SendSSEChat(InChat chatInput, EventHandler<List<ChatMessage>> eventHandler)
        {
            int speed = 100;
            string diastr = "";
            ChatMessage message = new ChatMessage("assistant");
            chatInput.Messages.Add(message);

            chatInput.Stream = true;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(chatInput), Encoding.UTF8))
                {
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

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

                                    var dia = JsonHelper.DeserializeJsonToObject<OutChat>(str);

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
                    else
                    {
                    }
                }
            }
        }
    }
}