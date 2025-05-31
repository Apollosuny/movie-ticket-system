using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieTicketSystem.Pages.Seats
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public EditModel(MovieTicketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seat Seat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(m => m.SeatId == id);
            
            if (seat == null)
            {
                return NotFound();
            }
            
            Seat = seat;
            ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ScreenId"] = new SelectList(_context.Screens, "ScreenId", "Name");
                return Page();
            }

            _context.Attach(Seat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(Seat.SeatId))
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

        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.SeatId == id);
        }
    }
}
