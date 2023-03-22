using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFY.ChatGpt.Dto.InputDto
{
    public class InChat
    {
        public InChat(List<ChatMessage> messages)
        {
            Model = ChatServiceProvider.ServiceProvider.GetService<ChatOption>()!.ChatModel;
            Messages = messages;
        }

        /// <summary>
        /// 模型名称
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// 对话
        /// </summary>
        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// 0到2之间,越大越随机
        /// </summary>
        [JsonProperty("temperature")]
        public int? Temperature { get; set; } = 1;

        /// <summary>
        /// 采样集中度不建议使用使用Temperature即可
        /// </summary>
        [JsonProperty("top_p")]
        public int? TopP { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty("n")]
        public int? N { get; set; }

        /// <summary>
        /// 是否以流的形式接收
        /// </summary>
        [JsonProperty("stream")]
        public bool? Stream { get; set; } = false;

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// </summary>
        [JsonProperty("stop")]
        public string? Stop { get; set; }

        /// <summary>
        /// 请求消耗的最大代币数,
        /// </summary>
        [JsonProperty("max_tokens")]
        public int? MaxTokens { get; set; } = 1000;

        /// <summary>
        /// 介于-0.2至0.2之间,正值增加讨论新主题的可能
        /// </summary>
        [JsonProperty("presence_penalty")]
        public int? PresencePenalty { get; set; } = 0;

        /// <summary>
        /// 介于-0.2至0.2之间,正值降低重复对话内容的可能
        /// </summary>
        [JsonProperty("frequency_penalty")]
        public int? FrequencyPenalty { get; set; } = 0;

        /// <summary>
        /// 唯一的用户id 可以帮助openai监控滥用行为
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; } = "";
    }
}