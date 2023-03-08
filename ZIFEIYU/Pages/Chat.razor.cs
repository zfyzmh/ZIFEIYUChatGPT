using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using ZIFEIYU.Model;
using ZIFEIYU.Model.Dto.InputDto;
using ZIFEIYU.Model.Dto.OutDto;
using ZIFEIYU.Services;
using ZIFEIYU.util;

namespace ZIFEIYU.Pages
{
    public class ChatBase : ComponentBase
    {
        public ChatBase()
        {
        }

        [Inject]
        public ChatGPTServices ChatGPTServices { get; set; }

        [Inject]
        public ZFYDatabase ZFYDatabase { get; set; }

        [Inject]
        private IJSRuntime jsRuntime { get; set; }

        public MudTheme _theme = new();
        public bool _isDarkMode=true;

        public string ChatTheme { get; set; }

        public string HelperText { get; set; }

        public DialogueOutput DialogueOutput { get; set; }
        public MudTextField<string> singleLineReference;
        public List<DialogueMessage> Messages { get; set; } = new List<DialogueMessage>();

        /// <summary>
        /// d
        /// </summary>
        public int ChatId { get; set; }

        public bool _processing = false;

        protected override async Task OnInitializedAsync()
        {
            ChatEntity chat = await ChatGPTServices.GetChatCurrent();
            if (chat != null)
            {
                ChatId = chat.Id;
                ChatTheme = chat.Theme;
                Messages = await JsonHelper.DeserializeJsonToListAsync<DialogueMessage>(chat.DialogJson);
            }

            await base.OnInitializedAsync();
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                Messages.Add(new DialogueMessage() { Role = "user", Content = HelperText });
                HelperText = "";
                _processing = true;
                StateHasChanged();
                UpdateScroll("IndexBody");
                await ChatGPTServices.SendSSEDialogue(new DialogueInput(Messages), DialogEvent);
                //Messages.Add(DialogueOutput.Choices[0].Message);
                StateHasChanged();
                UpdateScroll("IndexBody");
                _processing = false;

                await ChatGPTServices.SaveChat(Messages, ChatId);
            }
        }
        public async Task Reset()
        {
            HelperText=string.Empty;
            ChatId = 0;
            Messages = new List<DialogueMessage>();
            StateHasChanged();
        }
        
        private void DialogEvent(object sender, List<DialogueMessage> e)
        {
            Messages = e;
            StateHasChanged();
        }

        public async Task Test()
        {
        }

        private void UpdateScroll(string id)
        {
            jsRuntime.InvokeAsync<object>("updateScroll", id);
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