using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketSystem.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Họ tên là bắt buộc")]
            [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 3)]
            [Display(Name = "Họ tên")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    FullName = Input.FullName
                    // CreatedAt được tự động đặt giá trị trong constructor
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã tạo tài khoản mới với mật khẩu.");

                    // Kiểm tra và tạo role "User" nếu chưa có
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    // Thêm người dùng vào role "User"
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
                foreach (var error in result.Errors)
                {
                    var errorMessage = TranslateErrorMessage(error.Code, error.Description);
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        // Helper method to translate Identity error messages to Vietnamese
        private string TranslateErrorMessage(string errorCode, string defaultMessage)
        {
            return errorCode switch
            {
                "DuplicateUserName" => "Tên người dùng đã tồn tại.",
                "DuplicateEmail" => "Email đã được sử dụng.",
                "InvalidUserName" => "Tên người dùng không hợp lệ, chỉ có thể chứa chữ cái hoặc chữ số.",
                "PasswordTooShort" => "Mật khẩu phải có ít nhất 6 ký tự.",
                "PasswordRequiresNonAlphanumeric" => "Mật khẩu phải chứa ít nhất một ký tự đặc biệt.",
                "PasswordRequiresDigit" => "Mật khẩu phải chứa ít nhất một chữ số ('0'-'9').",
                "PasswordRequiresLower" => "Mật khẩu phải chứa ít nhất một chữ cái viết thường ('a'-'z').",
                "PasswordRequiresUpper" => "Mật khẩu phải chứa ít nhất một chữ cái viết hoa ('A'-'Z').",
                "PasswordRequiresUniqueChars" => "Mật khẩu phải chứa ít nhất {0} ký tự khác nhau.",
                _ => defaultMessage
            };
        }
    }
}
