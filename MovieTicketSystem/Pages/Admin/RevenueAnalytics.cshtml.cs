using Microsoft.AspNetCore.Authorization;
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

namespace MovieTicketSystem.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class RevenueAnalyticsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public RevenueAnalyticsModel(MovieTicketContext context)
        {
            _context = context;
        }

        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal DailyRevenue { get; set; }
        
        public int TotalBookings { get; set; }
        public int MonthlyBookings { get; set; }
        public int WeeklyBookings { get; set; }
        public int DailyBookings { get; set; }
        
        public Dictionary<string, decimal> RevenueByMovie { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, string> FormattedRevenueByMovie { get; set; } = new Dictionary<string, string>();
        
        // Debug properties
        public string DebugInfo { get; set; } = string.Empty;
        public string QueryInfo { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        
        public async Task OnGetAsync()
        {
            var now = DateTime.Now;
            StartDate ??= now.AddDays(-30);
            EndDate ??= now;

            // Debug info
            QueryInfo = $"Query Parameters: StartDate={StartDate:yyyy-MM-dd}, EndDate={EndDate:yyyy-MM-dd}, Status='Paid'";

            // Get current month's first day and last day
            var currentMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = currentMonth.AddMonths(1).AddDays(-1);

            try 
            {
                // Get all bookings with their showtimes and movies for the current month
                var query = _context.Bookings
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s != null ? s.Movie : null)
                    .Where(b => b.BookingTime >= StartDate && b.BookingTime <= EndDate && b.Status == "Completed");
                
                // Debug info to see the query SQL
                QueryInfo += $"\nQuery SQL: {query.ToQueryString()}";
                
                var bookings = await query.ToListAsync();
                
                // Debug info for results
                DebugInfo = $"Found {bookings?.Count ?? 0} bookings. ";
                
                if (bookings == null || !bookings.Any())
                {
                    // Check if there are any bookings at all (ignoring filters)
                    var totalBookingsCount = await _context.Bookings.CountAsync();
                    var completedBookingsCount = await _context.Bookings.CountAsync(b => b.Status == "Completed");
                    var pendingBookingsCount = await _context.Bookings.CountAsync(b => b.Status == "Pending");
                    var cancelledBookingsCount = await _context.Bookings.CountAsync(b => b.Status == "Cancelled");
                    
                    DebugInfo += $"Total bookings in DB: {totalBookingsCount}. " +
                        $"Completed: {completedBookingsCount}, " +
                        $"Pending: {pendingBookingsCount}, " +
                        $"Cancelled: {cancelledBookingsCount}.\n";
                    
                    DebugInfo += $"Total bookings in DB: {totalBookingsCount}. " +
                        $"Completed: {completedBookingsCount}, " +
                        $"Pending: {pendingBookingsCount}, " +
                        $"Cancelled: {cancelledBookingsCount}.\n";
                    
                    // Handle the case when no bookings are found
                    TotalRevenue = 0;
                    MonthlyRevenue = 0;
                    WeeklyRevenue = 0;
                    DailyRevenue = 0;
                    TotalBookings = 0;
                    MonthlyBookings = 0;
                    WeeklyBookings = 0;
                    DailyBookings = 0;
                    return;
                }
                
                DebugInfo += $"Valid bookings with proper relations: {bookings.Count(b => b.Showtime?.Movie != null)}";

                // Calculate total revenue
                TotalRevenue = bookings.Sum(b => b.TotalPrice);
                
                // Today's revenue
                var today = DateTime.Today;
                DailyRevenue = bookings
                    .Where(b => b.BookingTime.Date == today)
                    .Sum(b => b.TotalPrice);
                    
                // This week's revenue (last 7 days)
                var weekStart = now.AddDays(-7);
                WeeklyRevenue = bookings
                    .Where(b => b.BookingTime >= weekStart)
                    .Sum(b => b.TotalPrice);
                    
                // This month's revenue
                var monthStart = new DateTime(now.Year, now.Month, 1);
                MonthlyRevenue = bookings
                    .Where(b => b.BookingTime >= monthStart)
                    .Sum(b => b.TotalPrice);

                // Calculate booking counts
                TotalBookings = bookings.Count;
                DailyBookings = bookings.Count(b => b.BookingTime.Date == today);
                WeeklyBookings = bookings.Count(b => b.BookingTime >= weekStart);
                MonthlyBookings = bookings.Count(b => b.BookingTime >= monthStart);
                     // Get all movies from the database
            var allMovies = await _context.Movies.ToListAsync();
            
            // Revenue by movie for the current month - include all movies in the database
            var revenueData = bookings
                .Where(b => b.Showtime?.Movie != null && b.BookingTime >= monthStart && b.BookingTime <= lastDayOfMonth)
                .GroupBy(b => b.Showtime?.Movie?.Title ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => b.TotalPrice)
                );
            
            // Create a dictionary with all movies (including those with zero revenue)
            RevenueByMovie = new Dictionary<string, decimal>();
            
            // First add all movies from bookings
            foreach (var item in revenueData)
            {
                RevenueByMovie[item.Key] = item.Value;
            }
            
            // Then add any remaining movies from the database with zero revenue
            foreach (var movie in allMovies)
            {
                if (!RevenueByMovie.ContainsKey(movie.Title))
                {
                    RevenueByMovie[movie.Title] = 0;
                }
            }

            // Format the revenue in VND
            FormattedRevenueByMovie = RevenueByMovie
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => PriceFormatHelper.FormatPrice(kvp.Value)
                );
                
            // Add debug info about filtered movies
            DebugInfo += $"\nFound {RevenueByMovie.Count} movies in total, including {RevenueByMovie.Count(m => m.Value == 0)} with zero revenue";
            }
            catch (Exception ex)
            {
                DebugInfo = $"Error: {ex.Message}\n{ex.StackTrace}\nInner Exception: {ex.InnerException?.Message}";
            }
        }
    }
}
