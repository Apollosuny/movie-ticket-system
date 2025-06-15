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
    public class BookingDetailsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public BookingDetailsModel(MovieTicketContext context)
        {
            _context = context;
        }

        public Booking? Booking { get; set; }
        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();
        public IList<Payment> Payments { get; set; } = new List<Payment>();
        
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                return NotFound();
            }

            Booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s != null ? s.Movie : null)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s != null ? s.Screen : null)
                .FirstOrDefaultAsync(b => b.BookingId == Id);
                
            if (Booking == null)
            {
                return NotFound();
            }
            
            // Get tickets associated with the booking
            Tickets = await _context.Tickets
                .Include(t => t.Seat)
                .Where(t => t.BookingId == Id)
                .ToListAsync();
                
            // Get payments associated with the booking
            Payments = await _context.Payments
                .Where(p => p.BookingId == Id)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
                
            return Page();
        }
        
        public async Task<IActionResult> OnPostCancelBookingAsync()
        {
            var booking = await _context.Bookings.FindAsync(Id);
            
            if (booking == null)
            {
                return NotFound();
            }
            
            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            
            return RedirectToPage();
        }
    }
}
