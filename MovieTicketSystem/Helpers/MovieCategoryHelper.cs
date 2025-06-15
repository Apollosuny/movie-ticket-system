using System;
using System.Collections.Generic;
using System.Linq;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Helpers
{
    public static class MovieCategoryHelper
    {
        /// <summary>
        /// Determines if a movie is currently showing based on its release date and showtimes
        /// </summary>
        /// <param name="movie">The movie to check</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>True if the movie is currently showing, false otherwise</returns>
        public static bool IsNowShowing(this Movie movie, DateTime? currentDate = null) 
        {
            var date = currentDate ?? DateTime.Now;
            
            // A movie is "Now Showing" if:
            // 1. It has been released (release date is in the past or today)
            // 2. AND it has at least one showtime that is today or in the future
            return movie.ReleaseDate <= date && 
                  (movie.Showtimes != null && movie.Showtimes.Any(s => s.StartTime >= date.Date));
        }
        
        /// <summary>
        /// Determines if a movie is coming soon based on its release date and showtimes
        /// </summary>
        /// <param name="movie">The movie to check</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>True if the movie is coming soon, false otherwise</returns>
        public static bool IsComingSoon(this Movie movie, DateTime? currentDate = null)
        {
            var date = currentDate ?? DateTime.Now;
            
            // A movie is "Coming Soon" if:
            // 1. It has a future release date
            // OR
            // 2. It has been released but has no current or future showtimes
            return movie.ReleaseDate > date || 
                  (movie.Showtimes == null || !movie.Showtimes.Any(s => s.StartTime >= date.Date));
        }
        
        /// <summary>
        /// Gets a list of movies that are currently showing
        /// </summary>
        /// <param name="movies">List of movies to filter</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>List of movies that are currently showing</returns>
        public static List<Movie> GetNowShowingMovies(this IEnumerable<Movie> movies, DateTime? currentDate = null)
        {
            return movies.Where(m => m.IsNowShowing(currentDate)).ToList();
        }
        
        /// <summary>
        /// Gets a list of movies that are coming soon
        /// </summary>
        /// <param name="movies">List of movies to filter</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>List of movies that are coming soon</returns>
        public static List<Movie> GetComingSoonMovies(this IEnumerable<Movie> movies, DateTime? currentDate = null)
        {
            // Get movies that are not showing now
            return movies.Where(m => m.IsComingSoon(currentDate)).ToList();
        }
    }
}
