using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFY.ChatGpt.Dto.InputDto
{
    /// <summary>
    /// 创建图片
    /// </summary>
    public class InCreateImages
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="prompt"></param>
        public InCreateImages(string prompt)
        {
            Prompt = prompt;
        }

        /// <summary>
        /// 生成图片的文字
        /// </summary>
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        /// <summary>
        /// 生成数量
        /// </summary>
        [JsonProperty("n")]
        public int N { get; set; } = 1;

        /// <summary>
        /// 生成的图像的大小。必须是空或256x256 512x512 1024x1024之一。
        /// </summary>
        [JsonProperty("size")]
        public string Size { get; set; } = "256x256";

        /// <summary>
        /// 返回格式必须是url,b64_json之一。
        /// </summary>
        [JsonProperty("response_format")]
        public string ResponseFormat { get; set; } = "url";
    }
}