using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFY.ChatGpt
{
    /// <summary>
    /// 全局静态变量
    /// </summary>
    public class ChatOption
    {
        /// <summary>
        ///
        /// </summary>
        public string ChatModel { get; set; } = "gpt-3.5-turbo";

        /// <summary>
        ///
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// 连接超时时间,单位秒
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        ///
        /// </summary>
        public bool IsProxy { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        public string ProxyAddress { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public int ProxyPort { get; set; }
    }
}