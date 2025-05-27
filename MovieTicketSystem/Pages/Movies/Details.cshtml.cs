using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;        public DetailsModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }

        public Movie Movie { get; set; } = default!;
        public IList<Showtime> Showtimes { get; set; } = new List<Showtime>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie is not null)
            {
                Movie = movie;

                // Get upcoming showtimes for this movie
                Showtimes = await _context.Showtimes
                    .Include(s => s.Screen)
                    .Where(s => s.MovieId == id && s.StartTime > DateTime.Now)
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();

                return Page();
            }

            return NotFound();
        }
    }
}
