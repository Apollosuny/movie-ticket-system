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
        
        [BindProperty]
        public PaymentViewModel PaymentInfo { get; set; } = new PaymentViewModel();
        
        public bool HasSavedCards { get; set; } = false;

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
            
            // Get user's saved payment cards
            var savedCards = await _context.PaymentCards
                .Where(c => c.UserId == userId)
                .ToListAsync();
            
            PaymentInfo.ExistingCards = savedCards;
            HasSavedCards = savedCards.Any();
            
            if (HasSavedCards)
            {
                var defaultCard = savedCards.FirstOrDefault(c => c.IsDefault);
                if (defaultCard != null)
                {
                    PaymentInfo.SelectedCardId = defaultCard.CardId;
                }
            }
            
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

            // Handle card information
            string cardInfo = string.Empty;

            if (PaymentInfo.SelectedCardId.HasValue)
            {
                // User selected an existing card
                var selectedCard = await _context.PaymentCards
                    .FirstOrDefaultAsync(c => c.CardId == PaymentInfo.SelectedCardId && c.UserId == userId);
                
                if (selectedCard == null)
                {
                    ModelState.AddModelError("PaymentInfo.SelectedCardId", "Selected card not found.");
                    return Page();
                }
                
                cardInfo = $"{selectedCard.CardType} ending in {selectedCard.CardNumber.Substring(selectedCard.CardNumber.Length - 4)}";
            }
            else
            {
                // User is adding a new card
                if (string.IsNullOrEmpty(PaymentInfo.CardNumber) || 
                    string.IsNullOrEmpty(PaymentInfo.CardholderName) ||
                    string.IsNullOrEmpty(PaymentInfo.ExpiryDate) ||
                    string.IsNullOrEmpty(PaymentInfo.CVV))
                {
                    ModelState.AddModelError("PaymentInfo", "Please provide all card details.");
                    return Page();
                }
                
                cardInfo = $"{PaymentInfo.CardType} ending in {PaymentInfo.CardNumber.Substring(PaymentInfo.CardNumber.Length - 4)}";
                
                // Save the card if requested
                if (PaymentInfo.SaveCard)
                {
                    // If this is set as default, update all other cards to not be default
                    if (PaymentInfo.MakeDefault)
                    {
                        var existingDefaultCards = await _context.PaymentCards
                            .Where(c => c.UserId == userId && c.IsDefault)
                            .ToListAsync();
                        
                        foreach (var card in existingDefaultCards)
                        {
                            card.IsDefault = false;
                        }
                    }
                    
                    var newCard = new PaymentCard
                    {
                        UserId = userId!,
                        CardNumber = PaymentInfo.CardNumber,
                        CardholderName = PaymentInfo.CardholderName,
                        ExpiryDate = PaymentInfo.ExpiryDate,
                        CardType = PaymentInfo.CardType,
                        IsDefault = PaymentInfo.MakeDefault
                    };
                    
                    _context.PaymentCards.Add(newCard);
                }
            }

            // Create a new payment
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalPrice,
                Method = $"Online - {cardInfo}",
                PaidAt = DateTime.UtcNow
            };

            _context.Add(payment);
            booking.Status = "Completed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Payment successful!";
            return RedirectToPage("./Index");
        }
    }
}
