using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages
{
    public class MoviesListModel : PageModel
    {
        private readonly MovieTicketContext _context;

        public MoviesListModel(MovieTicketContext context)
        {
            _context = context;
        }

        public IList<Movie> Movies { get; set; } = new List<Movie>();

        public async Task OnGetAsync()
        {
            // Get movies from the database
            Movies = await _context.Movies
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }
    }
}
