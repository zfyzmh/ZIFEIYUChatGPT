using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFY.ChatGpt.Dto
{
    public class ChatMessage
    {
        public ChatMessage()
        { }

        public ChatMessage(string role)
        {
            Role = role;
            Content = "";
        }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}