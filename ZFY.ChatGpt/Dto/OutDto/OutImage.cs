using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFY.ChatGpt.Dto.OutDto
{
    public class OutImage
    {
        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}