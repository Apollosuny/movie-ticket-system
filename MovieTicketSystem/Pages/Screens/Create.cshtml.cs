using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketSystem.Data;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Screens
{
    public class CreateModel : PageModel
    {
        private readonly MovieTicketSystem.Data.MovieTicketContext _context;

        public CreateModel(MovieTicketSystem.Data.MovieTicketContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Screen Screen { get; set; } = default!;        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if there is already a screen with the same name
            var existingScreen = await _context.Screen
                .FirstOrDefaultAsync(s => s.Name.ToLower() == Screen.Name.ToLower());
            
            if (existingScreen != null)
            {
                ModelState.AddModelError("Screen.Name", "A screen with this name already exists");
            }

            if (Screen.SeatCapacity <= 0)
            {
                ModelState.AddModelError("Screen.SeatCapacity", "Seating capacity must be greater than zero");
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Screen.Add(Screen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Screen '{Screen.Name}' was created successfully!";
            return RedirectToPage("./Index");
        }
    }
}
