using CommunityToolkit.Maui.Alerts;
using Kotlin.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Http;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;
using ZFY.ChatGpt.Services;
using static Android.Icu.Text.CaseMap;

namespace ZIFEIYU.Services
{
    public class ImagesServices
    {
        private readonly ZFY.ChatGpt.Services.ImagesServices imagesServices;
        private readonly IHttpClientFactory _httpClientFactory;
        internal bool IsManualCancellation;

        public ImagesServices(ZFY.ChatGpt.Services.ImagesServices imagesServices, IHttpClientFactory httpClientFactory)
        {
            this.imagesServices = imagesServices;
            this._httpClientFactory = httpClientFactory;
        }

        public async Task<OutImage> CreateImages(InCreateImages inCreateImage)
        {
            return await imagesServices.CreateImages(inCreateImage);
        }

        public async Task Stop()
        {
            imagesServices.IsManualCancellation = true;
        }

        public async Task SaveImg(string url)
        {
            using HttpClient client = _httpClientFactory.CreateClient();

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using Stream stream = await response.Content.ReadAsStreamAsync();
                    string fileName = Path.GetFileName(url) + ".png";
                    string filePath = Path.Combine(await GetFilePath(), fileName); // 将文件保存到当前用户的下载文件夹中
                    using FileStream fileStream = new FileStream(filePath, FileMode.Create);

                    await stream.CopyToAsync(fileStream);
                    await Toast.Make($"图片已保存").Show();
                }
                else
                {
                    await Toast.Make($"下载图片失败，HTTP 状态码：{response.StatusCode}").Show();
                }
            }
            catch (Exception ex)
            {
                await Toast.Make($"下载图片出错：{ex.Message}").Show();
            }
        }

        private async Task<string> GetFilePath()
        {
            string filePath = string.Empty;

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // 在 Windows 中获取下载文件夹路径
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                filePath = Path.Combine(downloadsPath, "Downloads");
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var result = await FilePicker.PickAsync(PickOptions.Default);
                if (result != null)
                {
                    string cachePath = FileSystem.AppDataDirectory;.

                    .00
                    // 获取选择的文件夹路径
                    string folderPath = result.FullPath;
                    return folderPath;
                }
                else
                {
                    return FileSystem.AppDataDirectory;
                }
            }
            else
            {
                // 在其他平台中获取应用的缓存路径
                string cachePath = FileSystem.AppDataDirectory;
                filePath = cachePath;
            }

            return filePath;
        }
    }
}