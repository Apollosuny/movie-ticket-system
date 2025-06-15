using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Helpers;
using MovieTicketSystem.Models;
using System;
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
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "newest";
        
        [BindProperty(SupportsGet = true)]
        public string? Rating { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }
        
        public List<string?> AvailableRatings { get; private set; } = new List<string?>();
        
        public int TotalMovies { get; private set; }

        public async Task OnGetAsync()
        {
            // Get all available ratings
            AvailableRatings = await _context.Movies
                .Where(m => m.Rating != null)
                .Select(m => m.Rating)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
            
            // Create base query - include showtimes for categorization
            var moviesQuery = _context.Movies.Include(m => m.Showtimes).AsQueryable();
            
            // Apply search filter if provided
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                moviesQuery = moviesQuery.Where(m => 
                    m.Title.Contains(SearchTerm) || 
                    (m.Description != null && m.Description.Contains(SearchTerm)));
            }
            
            // Apply rating filter if provided
            if (!string.IsNullOrEmpty(Rating))
            {
                moviesQuery = moviesQuery.Where(m => m.Rating == Rating);
            }
            
            // Get movies for filtering before applying showing status
            var moviesForFiltering = await moviesQuery.ToListAsync();
            
            // Apply status filter if provided
            if (!string.IsNullOrEmpty(Status))
            {
                switch (Status.ToLower())
                {
                    case "showing":
                        // Use our helper method to filter for showing movies
                        Movies = moviesForFiltering.GetNowShowingMovies();
                        break;
                    case "comingsoon":
                        // Use our helper method to filter for coming soon movies
                        Movies = moviesForFiltering.GetComingSoonMovies();
                        break;
                    default:
                        Movies = moviesForFiltering;
                        break;
                }
            }
            else
            {
                Movies = moviesForFiltering;
            }
            
            // Store total count for stats
            TotalMovies = Movies.Count;
            
            // Apply sorting (now move this to the end since we've already fetched the movies)
            Movies = SortBy switch
            {
                "title_asc" => Movies.OrderBy(m => m.Title).ToList(),
                "title_desc" => Movies.OrderByDescending(m => m.Title).ToList(),
                "oldest" => Movies.OrderBy(m => m.ReleaseDate).ToList(),
                _ => Movies.OrderByDescending(m => m.ReleaseDate).ToList() // "newest" is default
            };
        }
    }
}
