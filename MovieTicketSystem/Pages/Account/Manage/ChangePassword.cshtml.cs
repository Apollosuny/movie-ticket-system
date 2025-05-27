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
            [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu hiện tại")]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
            [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        // Helper method to translate Identity error messages to Vietnamese
        private string TranslateErrorMessage(string errorCode, string defaultMessage)
        {
            return errorCode switch
            {
                "PasswordMismatch" => "Mật khẩu hiện tại không đúng.",
                "PasswordRequiresNonAlphanumeric" => "Mật khẩu phải chứa ít nhất một ký tự đặc biệt.",
                "PasswordRequiresDigit" => "Mật khẩu phải chứa ít nhất một chữ số ('0'-'9').",
                "PasswordRequiresLower" => "Mật khẩu phải chứa ít nhất một chữ cái viết thường ('a'-'z').",
                "PasswordRequiresUpper" => "Mật khẩu phải chứa ít nhất một chữ cái viết hoa ('A'-'Z').",
                "PasswordTooShort" => "Mật khẩu phải có ít nhất 6 ký tự.",
                "PasswordRequiresUniqueChars" => "Mật khẩu phải chứa ít nhất {0} ký tự khác nhau.",
                _ => defaultMessage
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
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
                return NotFound($"Không thể tải người dùng với ID '{_userManager.GetUserId(User)}'.");
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
            _logger.LogInformation("Người dùng đã thay đổi mật khẩu thành công.");
            StatusMessage = "Mật khẩu của bạn đã được thay đổi.";

            return RedirectToPage();
        }
    }
}
