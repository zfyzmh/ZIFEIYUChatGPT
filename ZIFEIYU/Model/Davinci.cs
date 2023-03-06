using Newtonsoft.Json;

namespace ZIFEIYU.Model
{
    /// <summary>
    /// 达芬奇提问输入模型
    /// </summary>
    public class DavinciInput
    {
        public DavinciInput()
        { }

        public DavinciInput(string prompt)
        {
            Prompt = prompt;
        }

        [JsonProperty("model")]
        public string Model { get; set; } = "text-davinci-003";

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("temperature")]
        public long Temperature { get; set; }

        [JsonProperty("max_tokens")]
        public long MaxTokens { get; set; } = 500;
    }

    /// <summary>
    /// 达芬奇输出模型
    /// </summary>
    public class DavinciOutput
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("choices")]
        public DavinciChoice[] Choices { get; set; }

        [JsonProperty("usage")]
        public Usage Usage { get; set; }
    }

    public class DavinciChoice
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }

        [JsonProperty("logprobs")]
        public object Logprobs { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }
}