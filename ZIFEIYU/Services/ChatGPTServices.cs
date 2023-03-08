using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System;
using ZIFEIYU.Model;
using ZIFEIYU.util;
using ZIFEIYU.Model.Dto.InputDto;
using ZIFEIYU.Model.Dto.OutDto;
using ZIFEIYU.Dao;
using SQLite;
using ZIFEIYU.Entity;

namespace ZIFEIYU.Services
{
    public class ChatGPTServices
    {
        private Dictionary<string, string> Headers = new Dictionary<string, string>();

        public event EventHandler<List<DialogueMessage>> StartPublishPaper;

        private readonly ChatDao _dao;

        public ChatGPTServices(ChatDao dao)
        {
            _dao = dao;
            Headers.Add("Authorization", "Bearer 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");
        }

        public async Task<DavinciOutput> GetDavinci(DavinciInput davinciInput)
        {
            return await HttpHelper.HttpPostAsync<DavinciOutput>("https://api.openai.com/v1/completions", JsonHelper.SerializeObject(davinciInput), headers: Headers);
        }

        /*JsonHelper.DeserializeJsonToList<DialogueMessage>(chat.DialogJson);

            */

        public async Task<ChatEntity> GetChatCurrent()
        {
            return await _dao.GetChatCurrent();
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

        public async Task SaveChat(List<DialogueMessage> messages, int id)
        {
            ChatEntity chatEntity = new ChatEntity();
            chatEntity.UpdateDate = DateTime.Now;
            chatEntity.Id = id;
            chatEntity.DialogJson = JsonHelper.SerializeObject(messages);
            chatEntity.Theme ??= messages.First().Content;
            await _dao.SaveChat(chatEntity);
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
                client.Timeout = new TimeSpan(0, 0, 15);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(dialogueInput), Encoding.UTF8))
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

                                    var dia = JsonHelper.DeserializeJsonToObject<DialogueOutput>(str);

                                    if (!string.IsNullOrEmpty(dia.Choices[0].Delta.Content))
                                    {
                                        diastr += dia.Choices[0].Delta.Content;
                                        message.Content = diastr;
                                        eventHandler.Invoke(this, dialogueInput.Messages);
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
        }
    }
}