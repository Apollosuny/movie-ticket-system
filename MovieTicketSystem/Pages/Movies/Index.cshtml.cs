using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;

        public IndexModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get;set; } = default!;        public async Task OnGetAsync()
        {
            Movie = await _context.Movies.ToListAsync();
        }
    }
}
