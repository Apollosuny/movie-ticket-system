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
            [Required(ErrorMessage = "Full name is required")]
            [StringLength(100, ErrorMessage = "{0} must be between {2} and {1} characters.", MinimumLength = 3)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "{0} must be between {2} and {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
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
                    _logger.LogInformation("User created a new account with password.");

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
                "DuplicateUserName" => "Username already exists.",
                "DuplicateEmail" => "Email is already in use.",
                "InvalidUserName" => "Invalid username, can only contain letters or digits.",
                "PasswordTooShort" => "Password must be at least 6 characters long.",
                "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character.",
                "PasswordRequiresDigit" => "Password must contain at least one digit ('0'-'9').",
                "PasswordRequiresLower" => "Password must contain at least one lowercase letter ('a'-'z').",
                "PasswordRequiresUpper" => "Password must contain at least one uppercase letter ('A'-'Z').",
                "PasswordRequiresUniqueChars" => "Password must contain at least {0} different characters.",
                _ => defaultMessage
            };
        }
    }
}
