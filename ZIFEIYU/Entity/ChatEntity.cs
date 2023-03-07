using SQLite;
using System.ComponentModel.DataAnnotations;

namespace ZIFEIYU.Entity
{
    /// <summary>
    /// 对话主表
    /// </summary>
    [Table("Chat")]
    public class ChatEntity
    {
        [Key] //主键
        public int Id { get; set; }

        /// <summary>
        /// 对话主题,默认为第一句话
        /// </summary>
        public String Theme { get; set; }

        public List<MessageEntity> Messages { get; set; } = new List<MessageEntity>();

        public DateTime? CreateDate { get; set; } = DateTime.Now;
    }
}