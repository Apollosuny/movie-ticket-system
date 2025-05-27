using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Seats
{
    public class DeleteModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public DeleteModel(MovieTicketContext context)
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
            else 
            {
                Seat = seat;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var seat = await _context.Seats.FindAsync(id);

            if (seat != null)
            {
                Seat = seat;
                _context.Seats.Remove(Seat);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
