using SQLite;
using System.ComponentModel.DataAnnotations;

namespace ZIFEIYU.Entity
{
    [Table("ChatMessage")]
    public class MessageEntity
    {
        [Key] //主键
        public int Id { get; set; }

        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string Role { get; set; }

        public string Content { get; set; }

        public ChatEntity chatEntity { get; set; }
        public int chatEntityId { get; set; }
    }
}