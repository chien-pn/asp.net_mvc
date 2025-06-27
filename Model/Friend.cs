using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_chat.Model
{
    public class Friend
    {
        [Required] public int Id { get; set; }
        
        [Column("user_id")]
        [Required] public int UserId { get; set; }

        [Column("friend_id")]
        [Required] public int FriendId { get; set; }

        [Column("status")]
        [Required] public int Status { get; set; } //Pending = 1, Accepted = 2, Blocked = 3

        [Column("created_at")]
        [Required] public string CreatedAt { get; set; } = "";
    }
}
