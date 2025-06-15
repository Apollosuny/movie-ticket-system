using Microsoft.AspNetCore.Authorization;
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
        public Dictionary<string, decimal> MonthlyRevenueData { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> WeeklyRevenueData { get; set; } = new Dictionary<string, decimal>();
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        
        public async Task OnGetAsync()
        {
            var now = DateTime.Now;
            StartDate ??= now.AddDays(-30);
            EndDate ??= now;

            // Get all bookings with their showtimes and movies
            var bookings = await _context.Bookings
                .Include(b => b.Showtime)
                    .ThenInclude(s => s != null ? s.Movie : null)
                .Where(b => b.BookingTime >= StartDate && b.BookingTime <= EndDate && b.Status == "Paid")
                .ToListAsync();

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
            
            // Revenue by movie
            RevenueByMovie = bookings
                .Where(b => b.Showtime?.Movie != null)
                .GroupBy(b => b.Showtime?.Movie?.Title ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => b.TotalPrice)
                );
                
            // Monthly revenue data (last 6 months)
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthName = monthNames[month.Month - 1];
                var firstDay = new DateTime(month.Year, month.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                
                var revenue = bookings
                    .Where(b => b.BookingTime >= firstDay && b.BookingTime <= lastDay)
                    .Sum(b => b.TotalPrice);
                    
                MonthlyRevenueData[$"{monthName} {month.Year}"] = revenue;
            }
            
            // Weekly revenue data (last 7 days)
            var dayNames = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            for (int i = 6; i >= 0; i--)
            {
                var date = now.Date.AddDays(-i);
                var dayOfWeek = ((int)date.DayOfWeek + 6) % 7; // Convert to Mon=0, Sun=6
                var dayName = dayNames[dayOfWeek];
                
                var revenue = bookings
                    .Where(b => b.BookingTime.Date == date)
                    .Sum(b => b.TotalPrice);
                    
                WeeklyRevenueData[dayName] = revenue;
            }
        }
    }
}
