using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZIFEIYU.Model;
using ZIFEIYU.Services;

namespace ZIFEIYU.Pages
{
    public class IndexBase : ComponentBase
    {
        public IndexBase()
        {
        }

        [Inject]
        public ChatGPTServices ChatGPTServices { get; set; }

        public string HelperText { get; set; }
        public string Text { get; set; }
        public DavinciOutput davinciOutput { get; set; }
        public MudTextField<string> singleLineReference;
        public List<DialogueMessage> Messages { get; set; } = new List<DialogueMessage>();
        public bool _processing = false;

        public async Task Send()
        {
            _processing = true;
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            Headers.Add("Authorization", "Bearer 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");
            davinciOutput = await ChatGPTServices.GetDavinci(new DavinciInput(HelperText));
            StateHasChanged();
            _processing = false;
        }
    }
}