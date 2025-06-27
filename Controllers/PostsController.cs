using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_chat.Enum;
using net_chat.Model.Request;
using net_chat.Model;
using net_chat.Model.Response;
using net_chat.Services;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace net_chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController(AppDbContext context, ILogger<PostsController> logger) : ControllerBase
    {

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return BadRequest("AccountId claim is missing or invalid.");
            }

            var post = new Post
            {
                AccountId = int.Parse(accountIdClaim.Value),
                Content = request.Content,
                ImageUrl = request.ImageUrl,
                Visibility = request.Visibility,
                GroupId = request.GroupId == 0 ? null : request.GroupId
            };
            if (request.GroupId.HasValue)
            {
                var groupExists = await context.Groups.AnyAsync(g => g.Id == request.GroupId.Value);
                if (!groupExists)
                    return BadRequest("Group does not exist.");
            }


            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(
                200,
                "Login successful",
                post
            ));
        }

        [HttpGet("GetAllPost")]
        [Authorize]
        public async Task<IActionResult> GetAllPost()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
                {
                    logger.LogWarning("Unauthorized access attempt: Missing or invalid user ID in token");
                    return Unauthorized(new ApiResponse<object>(401, "Invalid or missing user ID in token."));
                }

                logger.LogInformation("Fetching posts for user ID: {AccountId}", accountId);

                var posts = await context.Posts
                    .Where(p =>
                        p.Visibility == (int)PostVisibility.Public ||
                        (
                            p.AccountId == accountId || // My own posts
                            (p.Visibility == (int)PostVisibility.FriendsOnly &&
                             context.Friends.Any(f =>
                                 (f.UserId == accountId && f.FriendId == p.AccountId || f.FriendId == accountId && f.UserId == p.AccountId)
                                 && f.Status == (int)FriendType.Accepted
                             ))
                        )
                    )
                    .Include(p => p.Account)
                        .ThenInclude(a => a!.UserProfile)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                logger.LogInformation("Successfully retrieved {PostCount} posts for user {AccountId}", posts.Count, accountId);

                return Ok(new ApiResponse<List<Post>>(200, "Posts retrieved successfully.", posts));
            }
            catch (Exception ex)
            {
                // Log the exception (optional but recommended)
                logger.LogError(ex, "Error occurred while fetching posts: {ErrorMessage}", ex.Message);
                return StatusCode(500, new ApiResponse<List<Post>>(500, "An error occurred while fetching posts.", null, ex.Message));
            }
        }

        [HttpGet("GetMyPost")]
        [Authorize]
        public async Task<IActionResult> GetMyPost()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
                {
                    return Unauthorized(new ApiResponse<object>(401, "Invalid or missing user ID in token."));
                }
                var posts = await context.Posts
                       .Where(p => p.AccountId == accountId)
                       .Include(p => p.Account)
                           .ThenInclude(a => a!.UserProfile)
                       .OrderByDescending(p => p.CreatedAt)
                       .Select(p => new
                       {
                           p.Id,
                           p.Content,
                           p.Visibility,
                           p.CreatedAt,
                           Account = p.Account == null ? null : new
                           {
                               p.Account.Id,
                               p.Account.UserProfile
                           }
                       })
                       .ToListAsync();

                return Ok(new ApiResponse<object>(200, "My Post", posts));

            }
            catch (Exception ex)
            {
                // Log the exception (optional but recommended)
                return StatusCode(500, new ApiResponse<object>(500, "An error occurred while fetching posts.", null, ex.Message));
            }
        }
    }
}
