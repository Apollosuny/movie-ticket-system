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
    public class BookingManagementModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public BookingManagementModel(MovieTicketContext context)
        {
            _context = context;
        }
        
        public IList<Booking> Bookings { get; set; } = new List<Booking>();
        public PaginatedList<Booking>? PaginatedBookings { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? PageIndex { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int? PageSize { get; set; } = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s != null ? s.Movie : null)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.BookingTime)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchString))
            {
                bookingsQuery = bookingsQuery.Where(b => 
                    b.BookingId.ToString().Contains(SearchString) ||
                    (b.User != null && b.User.UserName != null && b.User.UserName.Contains(SearchString)) ||
                    (b.User != null && b.User.Email != null && b.User.Email.Contains(SearchString)) ||
                    (b.Showtime != null && b.Showtime.Movie != null && b.Showtime.Movie.Title != null && 
                     b.Showtime.Movie.Title.Contains(SearchString))
                );
            }
            
            if (!string.IsNullOrEmpty(Status))
            {
                bookingsQuery = bookingsQuery.Where(b => b.Status == Status);
            }
            
            if (StartDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.BookingTime.Date >= StartDate.Value.Date);
            }
            
            if (EndDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.BookingTime.Date <= EndDate.Value.Date);
            }

            // Create paginated list
            int pageSize = PageSize ?? 10;
            int pageIndex = PageIndex ?? 1;
            
            PaginatedBookings = await PaginatedList<Booking>.CreateAsync(
                bookingsQuery, pageIndex, pageSize);
                
            return Page();
        }
        
        public async Task<IActionResult> OnPostCancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            
            if (booking == null)
            {
                return NotFound();
            }
            
            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            
            return RedirectToPage();
        }
    }
    
    // Helper class for pagination
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;
            
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
