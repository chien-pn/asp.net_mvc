using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class Comment
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("content")]
        public string? Content { get; set; } = "";

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("user_profiles")]
        public UserProfile? UserProfile { get; set; }
    }

}
