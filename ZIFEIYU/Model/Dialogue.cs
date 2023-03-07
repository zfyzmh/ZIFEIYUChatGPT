using Newtonsoft.Json;

namespace ZIFEIYU.Model
{
    public class DialogueInput
    {
        public DialogueInput(List<DialogueMessage> messages)
        {
            Model = "gpt-3.5-turbo-0301";
            Messages = messages;
        }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("messages")]
        public List<DialogueMessage> Messages { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }
    }

    public class DialogueOutput
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("usage")]
        public Usage Usage { get; set; }

        [JsonProperty("choices")]
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        [JsonProperty("delta")]
        public DialogueMessage Delta { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }
    }
}