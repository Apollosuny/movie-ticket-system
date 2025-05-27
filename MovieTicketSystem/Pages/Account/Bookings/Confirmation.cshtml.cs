using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Account.Bookings
{
    [Authorize]
    public class ConfirmationModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public ConfirmationModel(MovieTicketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                    .ThenInclude(s => s!.Movie)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s!.Screen)
                .Include(b => b.Tickets!)
                    .ThenInclude(t => t.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == Booking.BookingId && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            // Create a new payment
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalPrice,
                Method = "Online",
                PaidAt = DateTime.UtcNow
            };

            _context.Add(payment);
            booking.Status = "Completed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thanh toán thành công!";
            return RedirectToPage("./Index");
        }
    }
}
