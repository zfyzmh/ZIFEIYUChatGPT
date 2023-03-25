using System.Collections.Generic;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;
using ZFY.ChatGpt.Services;
using ZIFEIYU.Dao;
using ZIFEIYU.Entity;
using ZIFEIYU.util;

namespace ZIFEIYU.Services
{
    public class ChatGPTServices
    {
        public bool IsManualCancellation;
        private Dictionary<string, string> Headers = new Dictionary<string, string>();

        public event EventHandler<List<ChatMessage>> StartPublishPaper;

        private readonly ChatDao _dao;
        private readonly ChatServices chatServices;

        public ChatGPTServices(ChatDao dao, ChatServices chatServices)
        {
            _dao = dao;
            this.chatServices = chatServices;
            Headers.Add("Authorization", "Bearer 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");
        }

        public async Task<OutChat> GetDavinci(DavinciInput davinciInput)
        {
            return await HttpHelper.HttpPostAsync<OutChat>("https://api.openai.com/v1/completions", JsonHelper.SerializeObject(davinciInput), headers: Headers);
        }

        public async Task<ChatEntity> GetChatCurrent()
        {
            return await _dao.GetChatCurrent();
        }

        public async Task SaveChat(List<ChatMessage> messages, long id)
        {
            ChatEntity chatEntity = new ChatEntity();
            chatEntity.UpdateDate = DateTime.Now;
            chatEntity.Id = id;
            chatEntity.DialogJson = JsonHelper.SerializeObject(messages);
            chatEntity.Theme ??= messages.First(m => m.Role == "user").Content;
            await _dao.SaveChat(chatEntity);
        }

        public async Task SendSSEChat(InChat chatInput, EventHandler<List<ChatMessage>> eventHandler)
        {
            await chatServices.SendSSEChat(chatInput, eventHandler);
        }

        public async Task SendChat(InChat chatInput, EventHandler<List<ChatMessage>> eventHandler)
        {
            int speed = 100;
            OutChat outChat = await chatServices.SendChat(chatInput);
            var chatmessagee = outChat.Choices[0].Message;

            if (IsManualCancellation) return;

            ChatMessage message = new ChatMessage("assistant");
            chatInput.Messages.Add(message);

            foreach (var item in chatmessagee.Content)
            {
                message.Content += item;
                eventHandler.Invoke(this, chatInput.Messages);

                if (speed >= 4) await Task.Delay(speed); speed -= 2;
            }
            return;
        }

        public async Task<List<ChatEntity>> InitHistory()
        {
            return (await _dao.GetAllChat()).OrderByDescending(m => m.UpdateDate).ToList();
        }

        public async Task Stop()
        {
            chatServices.IsManualCancellation = true;
        }

        /// <summary>
        /// 清理本地对话,保留十条最近的
        /// </summary>
        /// <returns></returns>
        public async Task ClearHistoryChat()
        {
            long[] clearIds = (await _dao.GetAllChat()).OrderByDescending(m => m.UpdateDate).Select(m => m.Id).ToArray();

            if (clearIds.Length > 10)
            {
                long[] clearIdss = clearIds.Skip(10).ToArray();
                await _dao.DeleteChat(clearIdss);
            }
        }

        public async Task DeleteChat(long chatId)
        {
            await _dao.DeleteChat(chatId);
        }
    }
}