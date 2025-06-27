using System.ComponentModel.DataAnnotations;

namespace net_chat.Model
{
    public class Messages
    {
        [Required] public int Id { get; set; }
        [Required] public int ConversationId { get; set; }
        [Required] public int SendId { get; set; }
        [Required] public int MessageType { get; set; }
        [Required] public string Content { get; set; } = "";
        [Required] public DateTime SentAt { get; set; }
        [Required] public DateTime ReadAt { get; set; }
        [Required] public bool IsDeleted { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }
    }
}
