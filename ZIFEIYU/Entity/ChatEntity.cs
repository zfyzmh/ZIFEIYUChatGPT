using SQLite;

namespace ZIFEIYU.Entity
{
    /// <summary>
    /// 对话主表
    /// </summary>
    [Table("Chat")]
    public class ChatEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// 对话主题,默认为第一句话
        /// </summary>
        public string? Theme { get; set; }

        public DateTime? CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; } = DateTime.Now;

        /// <summary>
        ///
        /// </summary>
        public string DialogJson { get; set; }
    }
}