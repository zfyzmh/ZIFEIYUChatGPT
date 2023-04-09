using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;

namespace ZFY.ChatGpt.Services
{
    public class ImagesServices
    {
        private readonly OpenAiHttpClientFactory _httpClientFactory;
        public bool IsManualCancellation;

        public ImagesServices(OpenAiHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 创建图像
        /// </summary>
        /// <param name="inCreateImage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<OutImage> CreateImages(InCreateImages inCreateImage)
        {
            IsManualCancellation = false;
            HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                using (HttpContent httpContent = new StringContent(JsonHelper.SerializeObject(inCreateImage), Encoding.UTF8, "application/json"))
                {
                    Task<HttpResponseMessage> taskResponse = client.PostAsync("/v1/images/generations", httpContent);

                    while (true)
                    {
                        await Task.Delay(100);
                        if (taskResponse.IsCompleted) break;
                        if (IsManualCancellation) throw new Exception("请求已取消");
                    }

                    var response = await taskResponse;
                    if (response.IsSuccessStatusCode)
                    {
                        return (await response.Content.ReadAsStringAsync()).ToEntity<OutImage>();
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("ApiKey未设置或已过期");
                    }
                    else
                    {
                        throw new Exception("访问chatgpt失败,状态码:" + (int)response.StatusCode + "错误信息:" + await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                string errprMessage = "网络连接超时,或请求已取消,请尝试以下方法解决\r\n" +
                    "1.检查网络设置\r\n" +
                    "2.设置更大的容忍超时时长\r\n" +
                    "3.在chatgpt访问量更少时使用";

                throw new Exception(errprMessage);
            }
            catch (Exception ex)
            {
                throw  ex;
            }
        }
    }
}