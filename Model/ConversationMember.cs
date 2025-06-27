using System.ComponentModel.DataAnnotations;

namespace net_chat.Model
{
    public class ConversationMember
    {
        [Required] public int Id { get; set; }
        [Required] public int ConversationId { get; set; }
        [Required] public int AccountId { get; set; }
        [Required] public bool IsAdmin { get; set; } = false;
        public Conversation? Conversation { get; set; }
    }
}
