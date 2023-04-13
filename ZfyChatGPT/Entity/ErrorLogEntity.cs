using SQLite;
using System.Diagnostics;

namespace ZfyChatGPT.Entity
{
    /// <summary>
    /// 错误日志
    /// </summary>
    [Table("ErrorLog")]
    public class ErrorLogEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public string Message { get; set; }
        public string StackTrace { get; set; }

        /// <summary>
        /// 类型为 UnhandledExceptionEventArgs
        /// </summary>
        public string ErrorInfo { get; set; }

        public DateTime ErrorTime { get; set; } = DateTime.Now;
    }
}