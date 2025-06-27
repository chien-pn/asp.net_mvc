using System.ComponentModel.DataAnnotations;
using net_chat.Enum;

namespace net_chat.Model.Request.Post
{
    public class CreateCommentRequest
    {
        [Required]
        public int IdPost { get; set; }

        [Required]
        public int ParentID { get; set; } = -1;

        [Required]
        public string Content { get; set; } = "";
    }
}
