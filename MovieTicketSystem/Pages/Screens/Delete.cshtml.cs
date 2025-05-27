using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Screens
{
    [Authorize(Roles = "Administrator")]
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

            var screen = await _context.Screens.FirstOrDefaultAsync(m => m.ScreenId == id);

            if (screen is not null)
            {
                Screen = screen;
                
                // Check if there are related showtimes or seats
                HasRelatedData = await _context.Showtimes.AnyAsync(s => s.ScreenId == id) ||
                                await _context.Seats.AnyAsync(s => s.ScreenId == id);

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

            var screen = await _context.Screens.FindAsync(id);
            
            if (screen != null)
            {
                // Check for related data before deleting
                var relatedShowtimes = await _context.Showtimes.Where(s => s.ScreenId == id).ToListAsync();
                var relatedSeats = await _context.Seats.Where(s => s.ScreenId == id).ToListAsync();
                
                // Remove related data first
                if (relatedShowtimes.Any())
                {
                    _context.Showtimes.RemoveRange(relatedShowtimes);
                }
                
                if (relatedSeats.Any())
                {
                    _context.Seats.RemoveRange(relatedSeats);
                }
                
                // Remove the screen
                _context.Screens.Remove(screen);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Screen '{screen.Name}' and all related data were successfully deleted.";
            }

            return RedirectToPage("./Index");
        }
    }
}
