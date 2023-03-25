using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using System.Linq;
using ZFY.ChatGpt;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Services;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using ZIFEIYU.Global;
using ZIFEIYU.Services;
using ZIFEIYU.util;
using JsonHelper = ZIFEIYU.util.JsonHelper;

namespace ZIFEIYU.Pages
{
    public partial class Chat : IDisposable
    {
        public bool _processing;

        private bool _isDispose = false;
        [Inject] public ChatGPTServices ChatGPTServices { get; set; }

        [Inject] public IJSRuntime jSRuntime { get; set; }

        public MudTextField<string> TextField { get; set; }
        public string ChatTheme { get; set; }

        public string HelperText { get; set; }

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public List<ChatEntity> chatEntities { get; set; }

        public long ChatId { get; set; }

        public bool PrepositiveVisible { get; set; }
        public string ChatPrepositive { get; set; }

        private DialogOptions PrepositiveDialogOptions = new() { FullWidth = true };

        protected override async Task OnInitializedAsync()
        {
            await InitHistory();
            ChatEntity chat = await ChatGPTServices.GetChatCurrent();
            if (chat != null)
            {
                ChatId = chat.Id;
                ChatTheme = chat.Theme!;
                Messages = await JsonHelper.DeserializeJsonToListAsync<ChatMessage>(chat.DialogJson);
            }

            await base.OnInitializedAsync();
        }

        private async Task InitHistory()
        {
            chatEntities = await ChatGPTServices.InitHistory();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                ChatGPTServices.IsManualCancellation = false;
                Messages.Add(new ChatMessage() { Role = "user", Content = HelperText });
                await TextField.Clear();
                await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                _processing = true;
                StateHasChanged();
                await ChatGPTServices.SendChat(new InChat(Messages), DialogEvent);
                _processing = false;
                if (!_isDispose) await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                StateHasChanged();

                if (ChatGPTServices.IsManualCancellation) return;

                await ChatGPTServices.SaveChat(Messages, ChatId);
                if (ChatId == 0)
                {
                    ChatEntity chat = await ChatGPTServices.GetChatCurrent();
                    if (chat != null)
                    {
                        ChatId = chat.Id;
                        ChatTheme = chat.Theme!;
                        Messages = await JsonHelper.DeserializeJsonToListAsync<ChatMessage>(chat.DialogJson);
                    }
                }
            }
        }

        public async Task Reset()
        {
            HelperText = string.Empty;
            ChatPrepositive = string.Empty;
            ChatId = 0;
            Messages = new List<ChatMessage>();
            StateHasChanged();
        }

        private void DialogEvent(object sender, List<ChatMessage> e)
        {
            Messages = e;
            StateHasChanged();
        }

        public async Task KeyDown(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Code == "Enter" && !_processing && !string.IsNullOrWhiteSpace(TextField.Value))
            {
                await Send();
            }
        }

        public async Task SwitchChat(long chatId)
        {
            if (chatId != ChatId)
            {
                ChatId = chatId;
                var chat = chatEntities.First(m => m.Id == ChatId);
                Messages = JsonHelper.DeserializeJsonToObject<List<ChatMessage>>(chat.DialogJson);
                await ChatGPTServices.SaveChat(Messages, chat.Id);
                StateHasChanged();
            }
        }

        public async Task Stop()
        {
            await Task.Delay(50);
            var lastMsg = Messages.Last();
            if (lastMsg.Role == "user")
            {
                ChatGPTServices.IsManualCancellation = true;
                await ChatGPTServices.Stop();
                HelperText = lastMsg.Content;
                Messages.Remove(lastMsg);
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            _isDispose = true;
        }

        public async Task ClearHistoryChat()
        {
            await ChatGPTServices.ClearHistoryChat();
            chatEntities = await ChatGPTServices.InitHistory();
            StateHasChanged();
        }

        public async Task DeleteChat()
        {
            await ChatGPTServices.DeleteChat(ChatId);
            ChatId = 0;
            Messages = new List<ChatMessage>();
            chatEntities = await ChatGPTServices.InitHistory();
            StateHasChanged();
        }

        public void AddPrepositive()
        {
            if (Messages.Count > 0)
            {
                if (Messages[0].Role != "system")
                {
                    Messages.Insert(0, new ChatMessage() { Role = "system", Content = ChatPrepositive });
                }
                else
                {
                    Messages[0].Content = ChatPrepositive;
                }
            }
            else
            {
                Messages.Insert(0, new ChatMessage() { Role = "system", Content = ChatPrepositive });
            }

            PrepositiveVisible = false;
        }

        public void Retry()
        {
            if (Messages.Count >= 2)
            {
                HelperText = Messages[Messages.Count - 2].Content;
                Messages.RemoveAt(Messages.Count - 1);
                Messages.RemoveAt(Messages.Count - 1);
            }
        }
    }
}