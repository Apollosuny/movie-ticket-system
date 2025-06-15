using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Seats
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public IndexModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IList<Seat> Seats { get; set; } = default!;
        public Screen? CurrentScreen { get; set; }

        public async Task OnGetAsync(int? screenId = null)
        {
            var query = _context.Seats.Include(s => s.Screen).AsQueryable();
            
            if (screenId.HasValue)
            {
                CurrentScreen = await _context.Screens.FirstOrDefaultAsync(s => s.ScreenId == screenId);
                query = query.Where(s => s.ScreenId == screenId);
            }
            
            Seats = await query.ToListAsync();
        }
    }
}
