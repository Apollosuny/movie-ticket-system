using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Screens
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;

        public EditModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Screen Screen { get; set; } = default!;
        
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasRelatedShowtimes { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screen = await _context.Screen.FirstOrDefaultAsync(m => m.ScreenId == id);
            if (screen == null)
            {
                return NotFound();
            }
            
            Screen = screen;
            
            // Check if there are any showtimes scheduled for this screen
            HasRelatedShowtimes = await _context.Showtime.AnyAsync(s => s.ScreenId == id);
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if there are any showtimes for this screen
            HasRelatedShowtimes = await _context.Showtime.AnyAsync(s => s.ScreenId == Screen.ScreenId);
            
            // Check if there's already a screen with the same name (excluding current screen)
            var existingScreen = await _context.Screen
                .FirstOrDefaultAsync(s => s.Name.ToLower() == Screen.Name.ToLower() 
                                       && s.ScreenId != Screen.ScreenId);
            
            if (existingScreen != null)
            {
                ModelState.AddModelError("Screen.Name", "A screen with this name already exists");
            }

            if (Screen.SeatCapacity <= 0)
            {
                ModelState.AddModelError("Screen.SeatCapacity", "Seating capacity must be greater than zero");
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the original screen to check if seat capacity is being reduced
            var originalScreen = await _context.Screen.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ScreenId == Screen.ScreenId);
                
            if (originalScreen != null && Screen.SeatCapacity < originalScreen.SeatCapacity && HasRelatedShowtimes)
            {
                // Check if any upcoming showtimes have bookings that would be affected
                var hasBookings = await _context.Showtime
                    .Where(s => s.ScreenId == Screen.ScreenId && s.StartTime > DateTime.Now)
                    .Include(s => s.Bookings)
                    .AnyAsync(s => s.Bookings != null && s.Bookings.Any());
                    
                if (hasBookings)
                {
                    ErrorMessage = "Cannot reduce seating capacity as there are upcoming showtimes with existing bookings.";
                    return Page();
                }
            }

            _context.Attach(Screen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Screen '{Screen.Name}' was updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScreenExists(Screen.ScreenId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ScreenExists(int id)
        {
            return _context.Screen.Any(e => e.ScreenId == id);
        }
    }
}
