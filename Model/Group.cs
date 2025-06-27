using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class Group
    {
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = "";

        [Column("description")]
        public string? Description { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Account? Owner { get; set; }

        public List<GroupMember>? Members { get; set; }
    }

}
