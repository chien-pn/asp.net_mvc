using System.ComponentModel.DataAnnotations;

namespace net_chat.Model
{
    public class Conversation
    {
        [Required] public int Id { get; set; }
        [Required] public int Type { get; set; } // private = 1, group = 2
        [Required] public string Name { get; set; } = "";
        [Required] public int CreateBy { get; set; }
        [Required] public string CreatedAt { get; set; } = "";
        public ICollection<ConversationMember>? Members { get; set; }
    }
}
