using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public UserInputModel UserInput { get; set; } = new UserInputModel();

        public List<string> AvailableRoles { get; set; } = new List<string>();
        
        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public class UserInputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Avatar URL")]
            public string? Avatar { get; set; }
        }

        public void OnGet()
        {
            // Get all available roles
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Get all available roles
                AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
                return Page();
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(UserInput.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already taken.");
                AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = UserInput.Email,
                Email = UserInput.Email,
                FullName = UserInput.FullName,
                PhoneNumber = UserInput.PhoneNumber,
                Avatar = UserInput.Avatar,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, UserInput.Password);

            if (result.Succeeded)
            {
                // Add selected roles to user
                if (SelectedRoles != null && SelectedRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, SelectedRoles);
                }
                else
                {
                    // Default to User role if none selected
                    await _userManager.AddToRoleAsync(user, "User");
                }

                return RedirectToPage("Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Get all available roles
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
            return Page();
        }
    }
}
