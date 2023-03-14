using Newtonsoft.Json;

namespace ZFY.ChatGpt.Dto.InputDto
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
}