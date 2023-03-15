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
        public static string ChatModel { get; } = "gpt-3.5-turbo-0301";

        /// <summary>
        ///
        /// </summary>
        public static string ApiKey { get; set; } = "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";
    }
}