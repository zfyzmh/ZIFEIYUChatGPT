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

        public string HelperText { get; set; }

        public DialogueOutput DialogueOutput { get; set; }
        public MudTextField<string> singleLineReference;
        public List<DialogueMessage> Messages { get; set; } = new List<DialogueMessage>();
        public bool _processing = false;

        protected override async Task OnInitializedAsync()
        {
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
            }
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