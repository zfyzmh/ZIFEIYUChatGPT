using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZIFEIYU.Entity
{
    /// <summary>
    /// 预制的对话模板
    /// </summary>
    [Table("Templates")]
    public class Templates
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [JsonProperty("act")]
        public string Act { get; set; }

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        public override string ToString() => Act;
    }
}