using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Helpers;
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
        public string MovieImageUrl { get; private set; } = string.Empty;

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
            
            // Thiết lập URL hình ảnh
            var defaultImage = ImageHelper.GetMoviePlaceholderUrl(Movie.Title ?? "Movie");
            MovieImageUrl = Movie.ImageBanner ?? defaultImage;

            // Get upcoming showtimes for this movie with the same date limit as ShowtimesList
            // MaxFutureDaysForShowtimes means today + the next X days, so we need to use <= to include the last day
            var maxDate = DateTime.Today.AddDays(DateConstants.MaxFutureDaysForShowtimes - 1); // -1 because it's 0-indexed (today counts as day 1)
            Showtimes = await _context.Showtimes
                .Include(s => s.Screen)
                .Where(s => s.MovieId == id && s.StartTime > DateTime.Now && s.StartTime.Date <= maxDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return Page();
        }
    }
}
