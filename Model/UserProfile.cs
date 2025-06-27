using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    [Table("user_profiles")]
    public class UserProfile
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; } = "";


        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("status_message")]
        public string? StatusMessage { get; set; } = "";

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public int Gender { get; set; }
    }
}
