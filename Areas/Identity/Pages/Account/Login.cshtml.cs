using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LibraryDB.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, 
                          UserManager<IdentityUser> userManager, 
                          ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();  // ✅ FIX CS8618 by initializing to a new object

        public string ReturnUrl { get; set; } = string.Empty;      // ✅ FIX CS8618 by initializing to empty string

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;      // ✅ FIX CS8618 by initializing to empty string

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;   // ✅ FIX CS8618 by initializing to empty string

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            await _signInManager.SignOutAsync();  // Optional: clears external cookies for clean login
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user != null) // ✅ FIX CS8604 by null-checking the user
                    {
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return LocalRedirect("/Admin/Dashboard");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "User"))
                        {
                            return LocalRedirect("/User/Dashboard");
                        }
                    }

                    return LocalRedirect(ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // Something failed → redisplay form
            return Page();
        }
    }
}
