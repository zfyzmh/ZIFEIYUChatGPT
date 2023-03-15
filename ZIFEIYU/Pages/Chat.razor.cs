using Microsoft.AspNetCore.Components;
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
        public ZFYDatabase ZFYDatabase { get; set; }

        public MudTheme _theme = new();
        public bool _isDarkMode = true;

        public string ChatTheme { get; set; }

        public string HelperText { get; set; }

        public OutChat DialogueOutput { get; set; }
        public MudTextField<string> singleLineReference;
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <summary>
        /// d
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
            JsCommon.UpdateScroll("IndexBody");
            return base.OnAfterRenderAsync(firstRender);
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                Messages.Add(new ChatMessage() { Role = "user", Content = HelperText });
                HelperText = "";
                _processing = true;
                StateHasChanged();
                await ChatGPTServices.SendSSEDialogue(new InChat(Messages), DialogEvent);
                //Messages.Add(DialogueOutput.Choices[0].Message);
                StateHasChanged();
                _processing = false;
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

        public async Task Test()
        {
        }

        /*public async Task KeyDown(KeyboardEventArgs keyboardEventArgs)
		{
			if (keyboardEventArgs.Code == "Enter" && !_processing) {
				await Send();
				HelperText = "";
				StateHasChanged();
			};
		}*/
    }
}