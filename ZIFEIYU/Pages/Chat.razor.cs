using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using ZIFEIYU.Global;
using ZIFEIYU.Services;
using ZIFEIYU.util;

namespace ZIFEIYU.Pages
{
    public partial class Chat : IDisposable
    {
        public bool _processing = false;

        private bool _isDispose = false;
        [Inject] public ChatGPTServices ChatGPTServices { get; set; }

        [Inject] public IJSRuntime jSRuntime { get; set; }
        public MudTextField<string> TextField { get; set; }
        public string ChatTheme { get; set; }

        public string HelperText { get; set; }

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public long ChatId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ChatEntity chat = await ChatGPTServices.GetChatCurrent();
            if (chat != null)
            {
                ChatId = chat.Id;
                ChatTheme = chat.Theme;
                Messages = await JsonHelper.DeserializeJsonToListAsync<ChatMessage>(chat.DialogJson);
            }

            await base.OnInitializedAsync();
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                Messages.Add(new ChatMessage() { Role = "user", Content = HelperText });
                await TextField.Clear();
                // await jSRuntime.InvokeAsync<Task>("ClearInput", "HelperText");
                await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                _processing = true;
                StateHasChanged();
                await ChatGPTServices.SendChat(new InChat(Messages), DialogEvent);
                _processing = false;
                // await jSRuntime.InvokeAsync<Task>("ClearInput", "HelperText");
                if (!_isDispose) await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                StateHasChanged();
                await ChatGPTServices.SaveChat(Messages, ChatId);
            }
        }

        public async Task Reset()
        {
            HelperText = string.Empty;
            ChatId = 0;
            Messages = new List<ChatMessage>();
            StateHasChanged();
        }

        private void DialogEvent(object sender, List<ChatMessage> e)
        {
            Messages = e;
            StateHasChanged();
        }

        public async void KeyDown(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Code == "Enter" && !_processing && !string.IsNullOrWhiteSpace(TextField.Value))
            {
                await Send();
            };
        }

        public void Dispose()
        {
            _isDispose = true;
        }
    }
}