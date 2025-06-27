using System.ComponentModel.DataAnnotations;
using net_chat.Enum;

namespace net_chat.Model.Request.Post
{
    public class CreatePostRequest
    {
        [Required]
        public string Content { get; set; } = "";
        [Required]
        public string? ImageUrl { get; set; }
        [Required]
        public int Visibility { get; set; } = (int)PostVisibility.Public;

        [Required] public int? GroupId { get; set; } = null;
    }

}
