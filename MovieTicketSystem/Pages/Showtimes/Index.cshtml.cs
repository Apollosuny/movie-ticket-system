using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Showtimes
{
    public class IndexModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public IndexModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IList<Showtime> Showtimes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .ToListAsync();
        }
    }
}
