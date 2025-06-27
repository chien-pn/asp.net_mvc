using System.ComponentModel.DataAnnotations;

namespace net_chat.Model.Request
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
