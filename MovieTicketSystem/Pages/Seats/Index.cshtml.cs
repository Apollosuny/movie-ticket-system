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

        public async Task OnGetAsync()
        {
            Seats = await _context.Seats
                .Include(s => s.Screen)
                .ToListAsync();
        }
    }
}
