using Newtonsoft.Json;

namespace ZIFEIYU.Model
{
    public class DialogueMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}