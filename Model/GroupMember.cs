using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class GroupMember
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("role")]
        public int Role { get; set; } = 1; // 1: Member, 2: Admin

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public Group? Group { get; set; }
        public Account? Account { get; set; }
    }

}
