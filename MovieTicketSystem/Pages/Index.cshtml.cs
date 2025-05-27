using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Collections.Generic;

namespace MovieTicketSystem.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MovieTicketContext _context;

    public IndexModel(ILogger<IndexModel> logger, MovieTicketContext context)
    {
        _logger = logger;
        _context = context;
    }    public int MovieCount { get; set; }
    public int ScreenCount { get; set; }
    public int ShowtimeCount { get; set; }
    public List<Movie> Movies { get; set; } = new List<Movie>();
    public List<Movie> ComingSoonMovies { get; set; } = new List<Movie>();

    public async Task OnGetAsync()
    {
        // Get dashboard statistics
        MovieCount = await _context.Movie.CountAsync();
        ScreenCount = await _context.Screen.CountAsync();
        ShowtimeCount = await _context.Showtime.CountAsync();
        
        // Try to get movies from the database
        var dbMovies = await _context.Movie.ToListAsync();
          // If no movies in the database, create mock data
        if (dbMovies == null || dbMovies.Count == 0)
        {
            Movies = CreateMockMovies();
            ComingSoonMovies = CreateMockComingSoonMovies();
        }
        else
        {
            // Use actual movies from database
            Movies = dbMovies.Where(m => m.ReleaseDate <= DateTime.Now).ToList();
            ComingSoonMovies = dbMovies.Where(m => m.ReleaseDate > DateTime.Now).ToList();
        }
    }
    
    private List<Movie> CreateMockMovies()
    {
        return new List<Movie>
        {
            new Movie 
            {
                MovieId = 1,
                Title = "Avengers: Final Saga",
                Duration = 152,
                Description = "The epic conclusion to the Avengers saga as heroes unite to defeat the universe's greatest threat.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(-30)
            },
            new Movie 
            {
                MovieId = 2,
                Title = "The Lost City",
                Duration = 118,
                Description = "An adventure film where an archaeologist discovers a hidden civilization with advanced technology.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(-15)
            },
            new Movie 
            {
                MovieId = 3,
                Title = "Midnight Shadows",
                Duration = 125,
                Description = "A thriller about a detective uncovering a conspiracy in a small town with a dark secret.",
                Rating = "R",
                ReleaseDate = DateTime.Now.AddDays(-7)
            },
            new Movie 
            {
                MovieId = 4,
                Title = "Love in Paris",
                Duration = 110,
                Description = "A romantic comedy about two strangers who meet in the City of Light and fall in love despite their differences.",
                Rating = "PG",
                ReleaseDate = DateTime.Now.AddDays(-5)
            },
            new Movie 
            {
                MovieId = 5,
                Title = "Space Pioneers",
                Duration = 135,
                Description = "A science fiction adventure about the first human colony on Mars facing unexpected challenges.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(-2)
            },
            new Movie 
            {
                MovieId = 6,
                Title = "The Last Dragon",
                Duration = 145,
                Description = "An epic fantasy tale about the last dragon and the warrior destined to protect it.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now
            }
        };
    }
    
    private List<Movie> CreateMockComingSoonMovies()
    {
        return new List<Movie>
        {
            new Movie 
            {
                MovieId = 7,
                Title = "Ocean Depths",
                Duration = 128,
                Description = "A deep sea exploration team discovers a new species with remarkable abilities, changing our understanding of evolution.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(10)
            },
            new Movie 
            {
                MovieId = 8,
                Title = "The Haunting",
                Duration = 115,
                Description = "A family moves into a new home, only to discover it's haunted by the ghosts of its former residents.",
                Rating = "R",
                ReleaseDate = DateTime.Now.AddDays(15)
            },
            new Movie 
            {
                MovieId = 9,
                Title = "Fast Track",
                Duration = 130,
                Description = "An action-packed racing film with high stakes and dangerous rivalries.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(30)
            },
            new Movie 
            {
                MovieId = 10,
                Title = "Digital Dreams",
                Duration = 140,
                Description = "A sci-fi thriller about a virtual reality experiment that blurs the line between the real world and digital consciousness.",
                Rating = "PG-13",
                ReleaseDate = DateTime.Now.AddDays(45)
            }
        };
    }
}
