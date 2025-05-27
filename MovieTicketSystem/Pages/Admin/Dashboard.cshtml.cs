using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class DashboardModel : PageModel
    {
        private readonly MovieTicketContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(
            MovieTicketContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int MovieCount { get; set; }
        public int ScreenCount { get; set; }
        public int ShowtimeCount { get; set; }
        public int UserCount { get; set; }
        public IList<Movie> LatestMovies { get; set; } = new List<Movie>();
        public IList<ApplicationUser> LatestUsers { get; set; } = new List<ApplicationUser>();

        public async Task OnGetAsync()
        {
            // Đếm số lượng các bản ghi
            MovieCount = await _context.Movie.CountAsync();
            ScreenCount = await _context.Screen.CountAsync();
            ShowtimeCount = await _context.Showtime.CountAsync();
            UserCount = await _userManager.Users.CountAsync();

            // Lấy 5 phim mới nhất
            LatestMovies = await _context.Movie
                .OrderByDescending(m => m.MovieId)
                .Take(5)
                .ToListAsync();

            // Lấy 5 người dùng mới nhất
            LatestUsers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
