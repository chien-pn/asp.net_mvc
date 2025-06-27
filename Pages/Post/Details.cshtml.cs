using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using net_chat.Model;
using net_chat.Services;

namespace net_chat.Pages.Post
{
    public class DetailsModel : PageModel
    {
        private readonly net_chat.Services.AppDbContext _context;

        public DetailsModel(net_chat.Services.AppDbContext context)
        {
            _context = context;
        }

        public Model.Post Post { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            else
            {
                Post = post;
            }
            return Page();
        }
    }
}
