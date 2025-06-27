using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using net_chat.Model.Request;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using net_chat.Model.Response;

namespace net_chat.Pages
{
    public class IndexModel(ILogger<HomeModel> logger) : PageModel
    {
        private readonly HttpClient _httpClient;
        [BindProperty] public LoginRequest Input { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Đăng nhập thất bại! Vui lòng kiểm tra lại thông tin.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Input.Email) || string.IsNullOrWhiteSpace(Input.Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return Page();
            }

            try
            {
                using var httpClient = new HttpClient();
                var apiUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/login";
                var response = await httpClient.PostAsJsonAsync(apiUrl, Input);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Email hoặc mật khẩu không đúng.";
                    return Page();
                }

                var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>();
                if (apiResult == null || apiResult.Data == null)
                {
                    ErrorMessage = "Đăng nhập thất bại! Dữ liệu trả về không hợp lệ.";
                    return Page();
                }
                var user = apiResult.Data.user;
                var token = apiResult.Data.token;

                // Ghi token vào session để sử dụng sau này
                HttpContext.Session.SetString("AccessToken", token);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.UserProfile?.UserName ?? user.Email),
                    new("AvatarUrl", user.UserProfile?.AvatarUrl ?? "image/user.png")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                ViewData["Token"] = token;
                ErrorMessage = null;
                return RedirectToPage("/Home", new { token = token });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Đăng ký thất bại!";
                return Page();
            }

            try
            {
                using var httpClient = new HttpClient();
                var apiUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/register";

                // Lấy dữ liệu từ form
                var username = $"{Request.Form["firstName"]} {Request.Form["firstName"]}";
                var email = Request.Form["email"].ToString();
                var password = Request.Form["password"].ToString();

                if (!DateTime.TryParse(Request.Form["birthDate"], out var birthDate))
                {
                    ErrorMessage = "Invalid birth date format.";
                    return Page();
                }

                var registerRequest = new RegisterRequest
                {
                    Username = username,
                    BirthDate = birthDate,
                    Email = email,
                    Password = password
                };

                var response = await httpClient.PostAsJsonAsync(apiUrl, registerRequest);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Đăng ký thất bại!";
                    return Page();
                }

                // Nếu muốn tự động đăng nhập sau khi đăng ký thành công, bạn có thể lấy token và user như phần login
                var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>();
                if (apiResult == null || apiResult.Data == null)
                {
                    ErrorMessage = "Đăng ký thất bại! Dữ liệu trả về không hợp lệ.";
                    return Page();
                }
                var user = apiResult.Data.user;
                var token = apiResult.Data.token;

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.UserProfile?.UserName ?? user.Email),
                    new("AvatarUrl", user.UserProfile?.AvatarUrl ?? "image/user.png")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
                ViewData["Token"] = token;
                ErrorMessage = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToPage("/Home", new { token = token });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
