using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
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
        
        public List<string> AvailableRatings { get; private set; } = new List<string>();
        
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
            
            // Create base query
            var moviesQuery = _context.Movies.AsQueryable();
            
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
            
            // Store total count for stats
            TotalMovies = await moviesQuery.CountAsync();
            
            // Apply sorting
            moviesQuery = SortBy switch
            {
                "title_asc" => moviesQuery.OrderBy(m => m.Title),
                "title_desc" => moviesQuery.OrderByDescending(m => m.Title),
                "oldest" => moviesQuery.OrderBy(m => m.ReleaseDate),
                _ => moviesQuery.OrderByDescending(m => m.ReleaseDate) // "newest" is default
            };
            
            // Get movies from the database
            Movies = await moviesQuery.ToListAsync();
        }
    }
}
