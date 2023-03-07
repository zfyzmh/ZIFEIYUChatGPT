using SQLite;

namespace ZIFEIYU.Entity
{
    [Table("ChatMessage")]
    public class MessageEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string Role { get; set; }

        public string Content { get; set; }

        public int ChatEntityId { get; set; }
    }
}