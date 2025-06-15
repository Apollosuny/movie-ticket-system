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
    public class ShowtimesListModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public ShowtimesListModel(MovieTicketContext context)
        {
            _context = context;
        }

        public DateTime SelectedDate { get; set; }
        public List<DateTime> Dates { get; set; } = new();
        public ILookup<Movie, Showtime> MovieShowtimes { get; set; } = 
            Enumerable.Empty<Showtime>().ToLookup(s => new Movie());

        public async Task OnGetAsync(string? date = null)
        {
            // Set selected date (default to today if not provided)
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                SelectedDate = parsedDate.Date;
            }
            else
            {
                SelectedDate = DateTime.Today;
            }

            // Generate a list of dates for the date picker based on our constant
            for (int i = 0; i < DateConstants.MaxFutureDaysForShowtimes; i++)
            {
                Dates.Add(DateTime.Today.AddDays(i));
            }

            // Get showtimes for the selected date
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .Where(s => s.StartTime.Date == SelectedDate.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Group showtimes by movie
            MovieShowtimes = showtimes
                .ToLookup(s => s.Movie!, s => s);
        }
    }
}
