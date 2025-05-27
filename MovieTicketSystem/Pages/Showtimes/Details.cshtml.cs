using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Showtimes
{
    public class DetailsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public DetailsModel(MovieTicketContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
