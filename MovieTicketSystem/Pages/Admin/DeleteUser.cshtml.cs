using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketSystem.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class DeleteUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser UserToDelete { get; set; } = null!;
        public List<string> UserRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            UserToDelete = await _userManager.FindByIdAsync(id);

            if (UserToDelete == null)
            {
                return NotFound();
            }

            // Get user roles
            UserRoles = (await _userManager.GetRolesAsync(UserToDelete)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserToDelete.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Check if it's not the only admin
            var adminRole = "Administrator";
            var isAdmin = await _userManager.IsInRoleAsync(user, adminRole);
            
            if (isAdmin)
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole);
                if (adminUsers.Count <= 1)
                {
                    ModelState.AddModelError(string.Empty, "Cannot delete the only administrator account.");
                    UserRoles = (await _userManager.GetRolesAsync(user)).ToList();
                    UserToDelete = user;
                    return Page();
                }
            }

            // Delete the user
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                UserRoles = (await _userManager.GetRolesAsync(user)).ToList();
                UserToDelete = user;
                return Page();
            }

            return RedirectToPage("Users");
        }
    }
}
