using System.Collections.Generic;
using System.Linq;
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
    public class IndexModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public IndexModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                Bookings = await _context.Bookings
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s!.Movie)
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s!.Screen)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BookingTime)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt vé.";
                return RedirectToPage();
            }

            if (booking.Status?.ToLower() != "pending")
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy các đơn đặt vé chưa thanh toán.";
                return RedirectToPage();
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã hủy đơn đặt vé thành công.";
            return RedirectToPage();
        }
    }
}
