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
    public class EditUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public ApplicationUser UserEdit { get; set; } = null!;

        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string> AvailableRoles { get; set; } = new List<string>();

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            UserEdit = await _userManager.FindByIdAsync(id);

            if (UserEdit == null)
            {
                return NotFound();
            }

            // Get current user roles
            UserRoles = (await _userManager.GetRolesAsync(UserEdit)).ToList();
            
            // Get all available roles
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Get all available roles
                AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
                
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserEdit.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FullName = UserEdit.FullName;
            user.Email = UserEdit.Email;
            user.UserName = UserEdit.Email; // Keep username same as email
            user.PhoneNumber = UserEdit.PhoneNumber;
            user.Avatar = UserEdit.Avatar;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                // Get all available roles
                AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
                
                return Page();
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove roles that are no longer selected
            var rolesToRemove = currentRoles.Where(r => !SelectedRoles.Contains(r));
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            // Add newly selected roles
            var rolesToAdd = SelectedRoles.Where(r => !currentRoles.Contains(r));
            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            return RedirectToPage("Users");
        }
    }
}
