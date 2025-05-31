using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieTicketSystem.Pages.Showtimes
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public EditModel(MovieTicketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Showtime Showtime { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(m => m.ShowtimeId == id);
            
            if (showtime == null)
            {
                return NotFound();
            }
            
            Showtime = showtime;
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
                ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
                return Page();
            }

            _context.Attach(Showtime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowtimeExists(Showtime.ShowtimeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ShowtimeExists(int id)
        {
            return _context.Showtimes.Any(e => e.ShowtimeId == id);
        }
    }
}
