using System.ComponentModel.DataAnnotations;

namespace net_chat.Model.Request
{
    public class RegisterInput
    {
        [Required(ErrorMessage = "Vui lòng nhập thông tin")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập thông tin")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Không thể bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Không thể bỏ trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 