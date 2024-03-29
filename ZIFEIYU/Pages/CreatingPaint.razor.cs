﻿using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using ZFY.ChatGpt.Dto.InputDto;
using ZIFEIYU.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ZIFEIYU.Pages
{
    public partial class CreatingPaint
    {
        public bool _processing;

        private bool _isDispose = false;
        [Inject] public ImagesServices? ImagesServices { get; set; }
        [Inject] public IMemoryCache? MemoryCache { get; set; }

        [Inject] public IJSRuntime? jSRuntime { get; set; }
        [Inject] public ISnackbar? Snackbar { get; set; }

        public string CreateImageTxt { get; set; }

        public int ImageNumber { get; set; } = 2;

        public string HelperText { get; set; }

        private List<string> Images = new List<string>();

        public bool DialogVisible { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitHistory();

            await base.OnInitializedAsync();
        }

        private async Task InitHistory()
        {
            if (MemoryCache.TryGetValue("Images", out List<string>? imgs))
            {
                Images = imgs!;
            }
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
                try
                {
                    ImagesServices.IsManualCancellation = false;
                    await CreateImage();
                    _processing = false;
                    if (!_isDispose) await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Snackbar.Add(ex.Message, Severity.Warning);
                }
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
            var outImage = await ImagesServices.CreateImages(new InCreateImages(HelperText) { N = ImageNumber });
            if (ImagesServices.IsManualCancellation) return;

            if (outImage != null)
            {
                Images.AddRange(outImage.Data.Select(m => m.Url).ToList());

                MemoryCache.Set("Images", Images);
                DialogVisible = false;
                _processing = false;
                StateHasChanged();
            }
        }

        public async Task Reset()
        {
            Images.Clear();
            StateHasChanged();
        }

        public async Task Stop()
        {
            await Task.Delay(50);

            ImagesServices.IsManualCancellation = true;
            await ImagesServices.Stop();
            _processing = false;
            StateHasChanged();
        }
    }
}