using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using net_chat.Model;
using net_chat.Model.Response;
using net_chat.Services;
using System.Security.Claims;
using Humanizer;
using System.Net.Http.Headers;

namespace net_chat.Pages
{
    [Authorize]
    public class HomeModel(AppDbContext context, ILogger<HomeModel> logger) : PageModel
    {
        public UserProfile UserProfile { get; set; } = new();
        public List<Model.Post> NewFeedList { get; set; } = [];


        public async Task<IActionResult> OnGetAsync()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
            {
                return RedirectToPage("/Index");
            }

            var userProfile = await context.UserProfiles
                                      .FirstOrDefaultAsync(up => up.AccountId == accountId);

            if (userProfile == null)
            {
                return NotFound($"Unable to load user with ID '{accountId}'.");
            }

            UserProfile = userProfile;

            try
            {
                using var httpClient = new HttpClient();
                var token = Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
                }
                // --- Xác định URL đúng 100 % ---
                var apiUrl = $"{Request.Scheme}://{Request.Host}/api/Posts/GetAllPost"; // chắc chắn đúng port?
                var req = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                req.Headers.Accept.Clear();
                req.Headers.Accept.Add(new("application/json"));

                var rsp = await httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

                if (rsp.IsSuccessStatusCode && rsp.Content.Headers.ContentType?.MediaType.Contains("json") == true)
                {
                    var apiResult = await rsp.Content.ReadFromJsonAsync<ApiResponse<List<Model.Post>>>();
                    if (apiResult != null && apiResult.Data != null)
                    {
                        NewFeedList = apiResult.Data;
                        logger.LogWarning("API DATA: {NewFeedList}", NewFeedList);
                    }
                    else
                    {
                        NewFeedList = [];
                        logger.LogWarning("API trả về JSON nhưng không có dữ liệu hợp lệ.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle HTTP request exceptions
                NewFeedList = [];
                logger.LogError(ex, "Error calling API to get posts: {ErrorMessage}", ex.Message);
            }

            return Page();
        }
    }
}
