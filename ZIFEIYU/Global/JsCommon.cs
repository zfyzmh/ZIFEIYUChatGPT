using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIFEIYU
{
    public class JsCommon
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        public async Task UpdateScroll(string divid)
        {
            await JS.InvokeVoidAsync("updateScroll", divid);
        }
    }
}