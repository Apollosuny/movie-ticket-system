using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages
{
    public class MovieDetailsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public MovieDetailsModel(MovieTicketContext context)
        {
            _context = context;
        }

        public Movie? Movie { get; set; }
        public List<Showtime> Showtimes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (Movie == null)
            {
                return Page();
            }

            // Get upcoming showtimes for this movie
            Showtimes = await _context.Showtimes
                .Include(s => s.Screen)
                .Where(s => s.MovieId == id && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return Page();
        }
    }
}
