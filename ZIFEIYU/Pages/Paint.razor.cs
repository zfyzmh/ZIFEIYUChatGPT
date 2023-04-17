using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using ZFY.ChatGpt.Dto.InputDto;
using ZIFEIYU.Services;

namespace ZIFEIYU.Pages
{
    public partial class Paint
    {
        public bool _processing;

        private bool _isDispose = false;
        [Inject] public ImagesServices ImagesServices { get; set; }

        [Inject] public IJSRuntime jSRuntime { get; set; }

        public string CreateImageTest { get; set; }

        public MudTextField<string> TextField { get; set; }

        public string HelperText { get; set; }

        public List<string> Images = new List<string>();

        public bool DialogVisible { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitHistory();

            await base.OnInitializedAsync();
        }

        private async Task InitHistory()
        {
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
                ImagesServices.IsManualCancellation = false;
                await CreateImage();
                await TextField.Clear();
                _processing = false;
                if (!_isDispose) await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                StateHasChanged();

                if (ImagesServices.IsManualCancellation) return;
            }
        }

        /// <summary>
        /// 创建图片
        /// </summary>
        /// <returns></returns>
        public async Task CreateImage()
        {
            DialogVisible = false;
            _processing = true;
            StateHasChanged();
            var outImage = await ImagesServices.CreateImages(new InCreateImages(HelperText));
            if (outImage != null)
            {
                Images.AddRange(outImage.Data.Select(m => m.Url).ToList());
                DialogVisible = false;
                _processing = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <returns></returns>
        public async Task LoadImage()
        {
        }

        public async Task Reset()
        {
            HelperText = string.Empty;
            Images.Clear();
            StateHasChanged();
        }

        public async Task KeyDown(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Code == "Enter" && !_processing && !string.IsNullOrWhiteSpace(TextField.Value))
            {
                await Send();
            }
        }

        public async Task Stop()
        {
            await Task.Delay(50);

            ImagesServices.IsManualCancellation = true;
            await ImagesServices.Stop();

            StateHasChanged();
        }

        public void Dispose()
        {
            _isDispose = true;
        }

        public async Task ClearHistory()
        {
            StateHasChanged();
        }

        public async Task Delete()
        {
            StateHasChanged();
        }

        public void SavePrepositive()
        {
        }

        public void Retry()
        {
        }
    }
}