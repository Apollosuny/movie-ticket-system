using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class DashboardModel : PageModel
    {
        private readonly MovieTicketContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(
            MovieTicketContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int MovieCount { get; set; }
        public int ScreenCount { get; set; }
        public int ShowtimeCount { get; set; }
        public int UserCount { get; set; }
        public int BookingsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public IList<Movie> LatestMovies { get; set; } = new List<Movie>();
        public IList<ApplicationUser> LatestUsers { get; set; } = new List<ApplicationUser>();
        public IList<Showtime> TodaysShowtimes { get; set; } = new List<Showtime>();
        public int ActiveShowtimesCount { get; set; }
        
        // For statistics display
        public Dictionary<string, int> WeeklyBookings { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new Dictionary<string, decimal>();

        public async Task OnGetAsync()
        {
            // Count records
            MovieCount = await _context.Movies.CountAsync();
            ScreenCount = await _context.Screens.CountAsync();
            ShowtimeCount = await _context.Showtimes.CountAsync();
            UserCount = await _userManager.Users.CountAsync();
            
            // Get latest 5 movies
            LatestMovies = await _context.Movies
                .OrderByDescending(m => m.MovieId)
                .Take(5)
                .ToListAsync();

            // Get latest 5 users
            LatestUsers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();
                
            // Get today's showtimes
            var today = DateTime.Today;
            TodaysShowtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .Where(s => s.StartTime.Date == today)
                .OrderBy(s => s.StartTime)
                .Take(6)
                .ToListAsync();
                
            // Count active showtimes (those that haven't ended yet)
            var now = DateTime.Now;
            ActiveShowtimesCount = await _context.Showtimes
                .Where(s => s.StartTime > now)
                .CountAsync();
                
            // Get weekly statistics (if you have Booking model)
            // This is a placeholder - adjust based on your actual models
            try {
                // Get real booking data
                BookingsCount = await _context.Bookings.CountAsync();
                TotalRevenue = await _context.Bookings.SumAsync(b => b.TotalPrice);
                
                // Get weekly booking data (last 7 days)
                var currentDate = DateTime.Today;
                var dayNames = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                
                // First try to get real data
                for (int i = 6; i >= 0; i--)
                {
                    var date = currentDate.AddDays(-i);
                    var dayOfWeek = ((int)date.DayOfWeek + 6) % 7; // Convert to Mon=0, Sun=6
                    var dayName = dayNames[dayOfWeek];
                    
                    var dayCount = await _context.Bookings
                        .Where(b => b.BookingTime.Date == date)
                        .CountAsync();
                        
                    var dayRevenue = await _context.Bookings
                        .Where(b => b.BookingTime.Date == date)
                        .SumAsync(b => b.TotalPrice);
                        
                    WeeklyBookings[dayName] = dayCount;
                    MonthlyRevenue[dayName] = dayRevenue;
                }
            }
            catch (Exception)
            {
                // Handle if Bookings table doesn't exist or other issues
                BookingsCount = 0;
                TotalRevenue = 0;
            }
        }
    }
}
