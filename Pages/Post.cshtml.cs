using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace net_chat.Pages
{
    public class PostModel(net_chat.Services.AppDbContext context) : PageModel
    {
        private readonly net_chat.Services.AppDbContext _context = context;

        public IList<Model.Post> Post { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Group).ToListAsync();

        }
    }
}
