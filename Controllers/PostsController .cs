using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_chat.Enum;
using net_chat.Model;
using net_chat.Model.Response;
using net_chat.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using net_chat.Model.Request.Post;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.Extensions.Hosting;

namespace net_chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController(AppDbContext context) : ControllerBase
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
                GroupId = request.GroupId == 0 ? null : request.GroupId,
                CreatedAt = DateTime.UtcNow
            };
            if (request.GroupId.HasValue && post.GroupId != null)
            {
                var groupExists = await context.Groups.AnyAsync(g => g.Id == post.GroupId);
                if (!groupExists)
                    return BadRequest("Group does not exist.");
            }

            post.Account = context.Accounts.FirstOrDefault(a => a.Id == int.Parse(accountIdClaim.Value))!;

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(
                200,
                "Login successful",
                post
            ));
        }

        [HttpGet("GetAllPost")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPost()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
                {
                    return Unauthorized(new ApiResponse<object>(401, "Invalid or missing user ID in token."));
                }

                // Lấy danh sách bạn bè
                var friendIds = await context.Friends
                    .Where(f =>
                        (f.UserId == accountId || f.FriendId == accountId) &&
                        f.Status == (int)FriendType.Accepted)
                    .Select(f => f.UserId == accountId ? f.FriendId : f.UserId)
                    .ToListAsync();

                // Lấy post trước
                var posts = await context.Posts
                    .Where(p =>
                        p.Visibility == (int)PostVisibility.Public ||
                        p.AccountId == accountId ||
                        (p.Visibility == (int)PostVisibility.FriendsOnly && friendIds.Contains(p.AccountId))
                    )
                    .Include(p => p.Account)
                    .ThenInclude(a => a!.UserProfile)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                // Lấy tất cả reaction của các post
                var postIds = posts.Select(p => p.Id).ToList();
                var reactions = await context.PostReactions
                    .Where(r => postIds.Contains(r.PostId))
                    .ToListAsync();
                var comments = await context.Comments.Where(c => postIds.Contains(c.PostId)).ToListAsync();

                // Gộp dữ liệu
                var result = posts.Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.Visibility,
                    p.CreatedAt,
                    Account = p.Account == null ? null : new
                    {
                        p.Account.Id,
                        p.Account.UserProfile
                    },
                    Reactions = reactions
                        .Where(r => r.PostId == p.Id)
                        .GroupBy(r => r.ReactionType)
                        .Select(g => new
                        {
                            ReactionType = g.Key.ToString(),
                            Count = g.Count()
                        })
                        .ToList(),
                    Comments = p.Comments = comments
                });

                return Ok(new ApiResponse<object>(200, "Posts retrieved successfully.", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "An error occurred while fetching posts.", null, ex.Message));
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

        [HttpPost("React")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ReactToPost([FromBody] ReactionRequest request)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out var accountId))
                {
                    return Unauthorized(new ApiResponse<object>(401, "Invalid or missing user ID in token."));
                }

                var existingReaction = await context.PostReactions
                    .FirstOrDefaultAsync(r => r.PostId == request.PostId && r.AccountId == accountId);

                if (existingReaction != null)
                {
                    if (existingReaction.ReactionType == request.ReactionType)
                    {
                        // Bỏ thích (Unreact)
                        context.PostReactions.Remove(existingReaction);
                    }
                    else
                    {
                        // Đổi loại biểu cảm
                        existingReaction.ReactionType = request.ReactionType;
                        existingReaction.CreatedAt = DateTime.UtcNow;
                        context.PostReactions.Update(existingReaction);
                    }
                }
                else
                {
                    // Thêm mới
                    var newReaction = new PostReaction
                    {
                        PostId = request.PostId,
                        AccountId = accountId,
                        ReactionType = request.ReactionType,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.PostReactions.Add(newReaction);
                }

                await context.SaveChangesAsync();
                return Ok(new ApiResponse<object>(200, "Reaction updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "An error occurred while reacting to post.", null, ex.Message));
            }
        }

        [HttpGet("GetComment")]
        [Authorize]
        public async Task<IActionResult> GetComment(int idPost)
        {
            var comments = await context.Comments.Where(c => c.PostId == idPost).ToListAsync();
            return Ok(new ApiResponse<object>(200, "Comments", comments));
        }

        [HttpPost("CreateComment")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest commentRequest)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
                {
                    return BadRequest("AccountId claim is missing or invalid.");
                }

                var userProfile = context.Accounts.Include(account => account.UserProfile).FirstOrDefault(a => a.Id == int.Parse(accountIdClaim.Value))!.UserProfile;

                if (userProfile == null)
                    return BadRequest("User claim is missing or invalid.");
                var comment = new Comment()
                {
                    PostId = commentRequest.IdPost,
                    UserId = userProfile.Id,
                    Content = commentRequest.Content,
                    ParentId = commentRequest.ParentID,
                    CreatedAt = DateTime.UtcNow,
                    UserProfile = userProfile
                };

                context.Comments.Add(comment);
                await context.SaveChangesAsync();
                return Ok(new ApiResponse<object>(200, "Comment successfully.", data: comment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while reacting to post.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

    }
}
