using Microsoft.AspNetCore.Mvc;
using net_chat.Model.Request;
using net_chat.Model.Response;
using net_chat.Services;

namespace net_chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(400, "Validation failed", null, ModelState));
                }

                var (account, token) = await authService.RegisterAsync(model);

                return Ok(new ApiResponse<object>(
                    200,
                    "Registration successful",
                    new
                    {
                        token,
                        user = new
                        {
                            account.Id,
                            account.Email,
                            account.CreatedAt,
                            account.UserProfile
                        }
                    }
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(
                    500,
                    "An error occurred during registration",
                    null,
                    ex.InnerException?.Message ?? ex.Message
                ));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(400, "Validation failed", null, ModelState));
                }
                
                var (account, token) = await authService.LoginAsync(model);

                return Ok(new ApiResponse<object>(
                    200,
                    "Login successful",
                    new
                    {
                        token = $"Bearer {token}",
                        user = new
                        {
                            account.Id,
                            account.Email,
                            account.CreatedAt,
                            account.UserProfile
                        }
                    }
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(
                    500,
                    "An error occurred during login",
                    null,
                    ex.InnerException?.Message ?? ex.Message
                ));
            }
        }
    }
    // Update the AvatarUrl property to allow nulls in the UserProfile class definition.
}
