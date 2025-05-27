using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MovieTicketContext _context;

    public IndexModel(ILogger<IndexModel> logger, MovieTicketContext context)
    {
        _logger = logger;
        _context = context;
    }

    public int MovieCount { get; set; }
    public int ScreenCount { get; set; }
    public int ShowtimeCount { get; set; }

    public async Task OnGetAsync()
    {
        // Get dashboard statistics
        MovieCount = await _context.Movie.CountAsync();
        ScreenCount = await _context.Screen.CountAsync();
        ShowtimeCount = await _context.Showtime.CountAsync();
    }
}
