using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Showtimes
{
    public class DetailsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public DetailsModel(MovieTicketContext context)
        {
            _context = context;
        }

        public Showtime Showtime { get; set; } = default!;
        public Dictionary<string, List<SeatViewModel>> SeatRows { get; set; } = new();

        [BindProperty]
        public string SelectedSeats { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public class SeatViewModel
        {
            public int SeatId { get; set; }
            public string Row { get; set; } = string.Empty;
            public int Number { get; set; }
            public string Type { get; set; } = string.Empty;
            public bool IsBooked { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // Set the Id property so it's available for the form submission
            Id = id;

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(m => m.ShowtimeId == id);
            
            if (showtime == null)
            {
                return NotFound();
            }
            
            Showtime = showtime;

            // Get all seats and booked seats in a single query
            var seats = await (from s in _context.Seats
                             where s.ScreenId == showtime.ScreenId
                             orderby s.Row, s.Number
                             select new
                             {
                                 Seat = s,
                                 IsBooked = _context.Tickets
                                    .Include(t => t.Booking)
                                    .Any(t => t.SeatId == s.SeatId && 
                                            t.Booking != null && 
                                            t.Booking.ShowtimeId == id && 
                                            t.Booking.Status != "Cancelled")
                             }).ToListAsync();

            // Group seats by row
            foreach (var seat in seats)
            {
                if (seat.Seat.Row == null) continue;
                
                var viewModel = new SeatViewModel
                {
                    SeatId = seat.Seat.SeatId,
                    Row = seat.Seat.Row,
                    Number = seat.Seat.Number,
                    Type = seat.Seat.Type ?? "Standard",
                    IsBooked = seat.IsBooked
                };

                if (!SeatRows.ContainsKey(seat.Seat.Row))
                {
                    SeatRows[seat.Seat.Row] = new List<SeatViewModel>();
                }
                SeatRows[seat.Seat.Row].Add(viewModel);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("./Details", new { id = Id }) });
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(m => m.ShowtimeId == Id);

            if (showtime == null)
            {
                return NotFound();
            }

            // Check if there's any selected seats
            if (string.IsNullOrEmpty(SelectedSeats))
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một ghế.");
                return await OnGetAsync(Id);
            }

            var selectedSeatIds = SelectedSeats.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("./Details", new { id = Id }) });
            }

            // Verify seats are not already booked
            var bookedSeats = await _context.Tickets
                .Include(t => t.Booking)
                .AnyAsync(t => t.Booking != null && 
                             t.Booking.ShowtimeId == Id && 
                             t.Booking.Status != "Cancelled" &&
                             selectedSeatIds.Contains(t.SeatId));

            if (bookedSeats)
            {
                ModelState.AddModelError("", "Một số ghế đã được đặt. Vui lòng chọn ghế khác.");
                return await OnGetAsync(Id);
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId ?? throw new InvalidOperationException("User ID is required"),
                ShowtimeId = Id.Value,
                BookingTime = DateTime.UtcNow,
                TotalPrice = showtime.Price * selectedSeatIds.Count,
                Status = "Pending",
                Tickets = selectedSeatIds.Select(seatId => new Ticket
                {
                    SeatId = seatId,
                    Price = showtime.Price
                }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Bookings/Confirmation", new { id = booking.BookingId });
        }
    }
}
