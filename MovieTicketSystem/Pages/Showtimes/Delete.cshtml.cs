using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Showtimes
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public DeleteModel(MovieTicketContext context)
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
            else 
            {
                Showtime = showtime;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var showtime = await _context.Showtimes.FindAsync(id);

            if (showtime != null)
            {
                Showtime = showtime;
                _context.Showtimes.Remove(Showtime);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
