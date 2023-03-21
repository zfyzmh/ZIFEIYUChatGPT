using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using ZIFEIYU.Global;
using ZIFEIYU.Services;
using ZIFEIYU.util;

namespace ZIFEIYU.Pages
{
    public partial class Chat
    {
        public Chat()
        {
        }

        public bool _processing = false;

        [Inject]
        public ChatGPTServices ChatGPTServices { get; set; }

        [Inject]
        public JsCommon JsCommon { get; set; }

        [Inject]
        public IJSRuntime jSRuntime { get; set; }

        [Inject]
        public ZFYDatabase ZFYDatabase { get; set; }

        public MudTheme _theme = new();
        public bool _isDarkMode = true;

        public MudTextField<string> TextField { get; set; }
        public string ChatTheme { get; set; }

        public string HelperText { get; set; }

        public OutChat DialogueOutput { get; set; }

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <summary>
        ///
        /// </summary>
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

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
            return base.OnAfterRenderAsync(firstRender);
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                Messages.Add(new ChatMessage() { Role = "user", Content = HelperText });
                await TextField.Clear();
                _processing = true;
                StateHasChanged();
                //await ChatGPTServices.SendSSEChat(new InChat(Messages), DialogEvent);
                //Messages.Add(DialogueOutput.Choices[0].Message);
                await ChatGPTServices.SendChat(new InChat(Messages), DialogEvent);

                _processing = false;
                await jSRuntime.InvokeAsync<Task>("ClearInput", "HelperText");
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

        public async void Test(string eventCallback)
        {
        }

        public async void KeyDown(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Code == "Enter" && !_processing && !string.IsNullOrWhiteSpace(TextField.Value))
            {
                await Send();
            };
        }
    }
}