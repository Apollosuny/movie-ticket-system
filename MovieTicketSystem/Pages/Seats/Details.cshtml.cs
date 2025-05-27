using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Seats
{
    public class DetailsModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public DetailsModel(MovieTicketContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
