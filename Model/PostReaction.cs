using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace net_chat.Model
{
    [Table("post_reactions")]
    public class PostReaction
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("reaction_type")]
        public int ReactionType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
