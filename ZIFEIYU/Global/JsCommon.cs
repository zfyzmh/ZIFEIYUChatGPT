using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ZIFEIYU
{
    public class JsCommon
    {
        [Inject]
        public IJSRuntime jSRuntime { get; set; }

        public async Task UpdateScroll(string divid)
        {
            await jSRuntime.InvokeAsync<Task>("updateScroll", divid);
        }

        public async Task Test()
        {
            await jSRuntime.InvokeVoidAsync("Test");
        }
    }
}