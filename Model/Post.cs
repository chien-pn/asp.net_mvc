using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class Post
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("account_id")]
        public int AccountId { get; set; }
        
        [Column("group_id")]
        public int? GroupId { get; set; }
        
        [Column("content")]
        public string Content { get; set; } = "";
        
        [Column("image_url")]
        public string? ImageUrl { get; set; }
        
        [Column("visibility")]
        public int Visibility { get; set; } = 1;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Account? Account { get; set; }
        public Group? Group { get; set; }
        public List<Comment>? Comments { get; set; }
    }


}
