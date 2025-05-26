using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Data
{
    public class MovieTicketContext : DbContext
    {
        public MovieTicketContext (DbContextOptions<MovieTicketContext> options)
            : base(options)
        {
        }

        public DbSet<MovieTicketSystem.Models.Movie> Movie { get; set; } = default!;
    }
}
