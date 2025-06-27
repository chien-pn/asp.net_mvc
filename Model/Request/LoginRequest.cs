using System.ComponentModel.DataAnnotations;

namespace net_chat.Model.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
