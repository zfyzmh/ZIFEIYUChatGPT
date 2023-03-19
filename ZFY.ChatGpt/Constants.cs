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
    public static class Constants
    {
        /// <summary>
        ///
        /// </summary>
        public static string ChatModel { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public static string ApiKey { get; set; } = "";

        /// <summary>
        /// 连接超时时间,单位秒
        /// </summary>
        public static int Timeout { get; set; } = 30;

        /// <summary>
        ///
        /// </summary>
        public static bool IsProxy { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        public static string ProxyAddress { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public static int ProxyPort { get; set; }
    }
}