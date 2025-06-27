using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class Account
    {
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = "";

        [Column("password_hash")]
        public string PasswordHash { get; set; } = "";

        [Column("created_at")]
        public string CreatedAt { get; set; } = "";

        [Column("user_profiles")]
        public UserProfile? UserProfile { get; set; }
    }
}
