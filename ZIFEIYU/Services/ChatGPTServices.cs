using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System;
using ZIFEIYU.Model;
using ZIFEIYU.util;
using ZIFEIYU.Model.Dto.InputDto;
using ZIFEIYU.Model.Dto.OutDto;

namespace ZIFEIYU.Services
{
    public class ChatGPTServices
    {
        private Dictionary<string, string> Headers = new Dictionary<string, string>();

        public event EventHandler<List<DialogueMessage>> StartPublishPaper;

        public ChatGPTServices()
        {
            // Headers.Add("Authorization", "Bearer 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");
            Headers.Add("Authorization", "Bearer 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");
        }

        public async Task<DavinciOutput> GetDavinci(DavinciInput davinciInput)
        {
            return await HttpHelper.HttpPostAsync<DavinciOutput>("https://api.openai.com/v1/completions", JsonHelper.SerializeObject(davinciInput), headers: Headers);
        }

        public async Task<DialogueOutput> SendDialogue(DialogueInput dialogueInput)
        {
            try
            {
                return await HttpHelper.HttpPostAsync<DialogueOutput>("https://api.openai.com/v1/chat/completions", JsonHelper.SerializeObject(dialogueInput), headers: Headers);
            }
            catch (Exception)
            {
                return new DialogueOutput();
            }
        }

        public async Task SendSSEDialogue(DialogueInput dialogueInput, EventHandler<List<DialogueMessage>> eventHandler)
        {
            int speed = 70;
            string diastr = "";
            DialogueMessage message = new DialogueMessage("assistant");
            dialogueInput.Messages.Add(message);

            dialogueInput.Stream = true;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 30);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(dialogueInput), Encoding.UTF8))
                {
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream Stream = response.Content.ReadAsStream())
                        {
                            StreamReader streamReader = new StreamReader(Stream);
                            while (true)
                            {
                                var str = streamReader.ReadLine();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    str = str.Remove(0, str.IndexOf('{'));

                                    var dia = JsonHelper.DeserializeJsonToObject<DialogueOutput>(str);

                                    if (!string.IsNullOrEmpty(dia.Choices[0].Delta.Content))
                                    {
                                        diastr += dia.Choices[0].Delta.Content;
                                        message.Content = diastr;
                                        eventHandler.Invoke(this, dialogueInput.Messages);
                                        await Task.Delay(speed--);
                                    }
                                    if (dia.Choices[0].FinishReason != null)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}