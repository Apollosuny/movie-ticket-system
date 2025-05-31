using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Current password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "New password is required")]
            [StringLength(100, ErrorMessage = "{0} must be between {2} and {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        // Helper method to translate Identity error messages
        private string TranslateErrorMessage(string errorCode, string defaultMessage)
        {
            return errorCode switch
            {
                "PasswordMismatch" => "Current password is incorrect.",
                "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character.",
                "PasswordRequiresDigit" => "Password must contain at least one digit ('0'-'9').",
                "PasswordRequiresLower" => "Password must contain at least one lowercase letter ('a'-'z').",
                "PasswordRequiresUpper" => "Password must contain at least one uppercase letter ('A'-'Z').",
                "PasswordTooShort" => "Password must be at least 6 characters long.",
                "PasswordRequiresUniqueChars" => "Password must contain at least {0} different characters.",
                _ => defaultMessage
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Cannot load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Cannot load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, TranslateErrorMessage(error.Code, error.Description));
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}
