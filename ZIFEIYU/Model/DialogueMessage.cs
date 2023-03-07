using Newtonsoft.Json;

namespace ZIFEIYU.Model
{
    public class DialogueMessage
    {
        public DialogueMessage() { }
        public DialogueMessage( string role)
        {
            Role=role;
            Content = "";
        }
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}