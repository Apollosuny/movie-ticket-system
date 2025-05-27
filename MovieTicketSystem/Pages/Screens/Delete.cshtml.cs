using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Screens
{
    public class DeleteModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;

        public DeleteModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Screen Screen { get; set; } = default!;
        
        public bool HasRelatedData { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screen = await _context.Screen.FirstOrDefaultAsync(m => m.ScreenId == id);

            if (screen is not null)
            {
                Screen = screen;
                
                // Check if there are related showtimes or seats
                HasRelatedData = await _context.Showtime.AnyAsync(s => s.ScreenId == id) ||
                                await _context.Seat.AnyAsync(s => s.ScreenId == id);

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screen = await _context.Screen.FindAsync(id);
            
            if (screen != null)
            {
                // Check for related data before deleting
                var relatedShowtimes = await _context.Showtime.Where(s => s.ScreenId == id).ToListAsync();
                var relatedSeats = await _context.Seat.Where(s => s.ScreenId == id).ToListAsync();
                
                // Remove related data first
                if (relatedShowtimes.Any())
                {
                    _context.Showtime.RemoveRange(relatedShowtimes);
                }
                
                if (relatedSeats.Any())
                {
                    _context.Seat.RemoveRange(relatedSeats);
                }
                
                // Remove the screen
                _context.Screen.Remove(screen);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Screen '{screen.Name}' and all related data were successfully deleted.";
            }

            return RedirectToPage("./Index");
        }
    }
}
