using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Showtimes
{
    public class CreateModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public CreateModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
            return Page();
        }

        [BindProperty]
        public Showtime Showtime { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
                ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
                return Page();
            }

            _context.Showtimes.Add(Showtime);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
