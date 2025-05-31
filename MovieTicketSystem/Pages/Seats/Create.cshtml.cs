using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Seats
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public CreateModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
            return Page();
        }

        [BindProperty]
        public Seat Seat { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
                return Page();
            }

            _context.Seats.Add(Seat);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
