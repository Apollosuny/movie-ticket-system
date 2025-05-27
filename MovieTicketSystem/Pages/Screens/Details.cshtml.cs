using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Screens
{
    public class DetailsModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;

        public DetailsModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }        public Screen Screen { get; set; } = default!;
        public List<Showtime> UpcomingShowtimes { get; set; } = new List<Showtime>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screen = await _context.Screen.FirstOrDefaultAsync(m => m.ScreenId == id);

            if (screen is not null)
            {
                Screen = screen;
                
                // Get upcoming showtimes for this screen
                UpcomingShowtimes = await _context.Showtime
                    .Include(s => s.Movie)
                    .Where(s => s.ScreenId == id && s.StartTime > DateTime.Now)
                    .OrderBy(s => s.StartTime)
                    .Take(5)  // Limit to 5 upcoming showtimes
                    .ToListAsync();

                return Page();
            }

            return NotFound();
        }
    }
}
