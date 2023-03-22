using Newtonsoft.Json;

namespace ZFY.ChatGpt.Dto
{
    public class Usage
    {
        /// <summary>
        /// 请求令牌
        /// </summary>
        [JsonProperty("prompt_tokens")]
        public long PromptTokens { get; set; }

        /// <summary>
        /// 回复消耗令牌
        /// </summary>
        [JsonProperty("completion_tokens")]
        public long CompletionTokens { get; set; }

        /// <summary>
        /// 消耗总令牌数
        /// </summary>
        [JsonProperty("total_tokens")]
        public long TotalTokens { get; set; }
    }
}