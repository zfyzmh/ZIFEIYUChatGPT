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
        public int Id { get; set; }

        /// <summary>
        /// 对话主题,默认为第一句话
        /// </summary>
        public String Theme { get; set; }

        public DateTime? CreateDate { get; set; } = DateTime.Now;
    }
}