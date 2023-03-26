using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZIFEIYU.Dto
{
    /// <summary>
    /// 预制的对话模板
    /// </summary>
    public class Templates
    {
        [JsonProperty("act")]
        public string Act { get; set; }

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        public override string ToString() => Act;
    }
}