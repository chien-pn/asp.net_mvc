using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using net_chat.Model;
using net_chat.Services;

namespace net_chat.Pages.Post
{
    public class CreateModel(net_chat.Services.AppDbContext context) : PageModel
    {
        public IActionResult OnGet()
        {
        ViewData["AccountId"] = new SelectList(context.Accounts, "Id", "Id");
        ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Model.Post Post { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            context.Posts.Add(Post);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
