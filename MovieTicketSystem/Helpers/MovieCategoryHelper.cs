using System;
using System.Collections.Generic;
using System.Linq;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Helpers
{
    public static class MovieCategoryHelper
    {
        /// <summary>
        /// Determines if a movie is currently showing based on its showtimes
        /// </summary>
        /// <param name="movie">The movie to check</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>True if the movie is currently showing, false otherwise</returns>
        public static bool IsNowShowing(this Movie movie, DateTime? currentDate = null) 
        {
            var date = currentDate ?? DateTime.Now;
            
            // A movie is "Now Showing" if it has at least one showtime today or in the future
            return movie.Showtimes != null && movie.Showtimes.Any(s => s.StartTime >= date.Date);
        }
        
        /// <summary>
        /// Determines if a movie is coming soon based on its showtimes
        /// </summary>
        /// <param name="movie">The movie to check</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>True if the movie is coming soon, false otherwise</returns>
        public static bool IsComingSoon(this Movie movie, DateTime? currentDate = null)
        {
            var date = currentDate ?? DateTime.Now;
            
            // A movie is "Coming Soon" if it has no showtimes
            return movie.Showtimes == null || !movie.Showtimes.Any(s => s.StartTime >= date.Date);
        }
        
        /// <summary>
        /// Gets a list of movies that are currently showing (have showtimes)
        /// </summary>
        /// <param name="movies">List of movies to filter</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>List of movies that are currently showing</returns>
        public static List<Movie> GetNowShowingMovies(this IEnumerable<Movie> movies, DateTime? currentDate = null)
        {
            return movies.Where(m => m.IsNowShowing(currentDate)).ToList();
        }
        
        /// <summary>
        /// Gets a list of movies that are coming soon (have no showtimes)
        /// </summary>
        /// <param name="movies">List of movies to filter</param>
        /// <param name="currentDate">Optional current date parameter for testing</param>
        /// <returns>List of movies that are coming soon</returns>
        public static List<Movie> GetComingSoonMovies(this IEnumerable<Movie> movies, DateTime? currentDate = null)
        {
            // Get movies that have no showtimes
            return movies.Where(m => m.IsComingSoon(currentDate)).ToList();
        }
    }
}
