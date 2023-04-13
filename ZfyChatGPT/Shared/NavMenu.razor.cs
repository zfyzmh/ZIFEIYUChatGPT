using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZfyChatGPT.Shared
{
    public partial class NavMenu
    {
        private bool open;
        private Anchor anchor;

        private void ToggleDrawer()
        {
            open = !open;
        }
    }
}