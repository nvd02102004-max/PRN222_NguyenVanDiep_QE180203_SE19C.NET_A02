using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public LoginModel(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToPage("/Admin/Accounts/Index");
                if (User.IsInRole("Staff"))
                    return RedirectToPage("/Staff/NewsArticle/Index");
                return RedirectToPage("/News/Index");
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var adminEmail = _configuration["AdminAccount:Email"] ?? "admin@FUNewsManagementSystem.org";
            var adminPassword = _configuration["AdminAccount:Password"] ?? "@@abc123@@";

            // 1. Check Admin Account from appsettings.json
            if (string.Equals(Input.Email.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase) &&
                Input.Password == adminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "0"),
                    new Claim(ClaimTypes.Name, "System Administrator"),
                    new Claim(ClaimTypes.Email, adminEmail),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties { IsPersistent = Input.RememberMe });

                HttpContext.Session.SetInt32("AccountId", 0);
                HttpContext.Session.SetString("AccountName", "System Administrator");
                HttpContext.Session.SetString("Role", "Admin");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToPage("/Admin/Accounts/Index");
            }

            // 2. Check Database Account
            var account = _accountService.Authenticate(Input.Email, Input.Password);
            if (account != null)
            {
                string roleName = account.AccountRole == 1 ? "Staff" : (account.AccountRole == 2 ? "Lecturer" : "Guest");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, account.AccountName ?? "User"),
                    new Claim(ClaimTypes.Email, account.AccountEmail ?? ""),
                    new Claim(ClaimTypes.Role, roleName)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties { IsPersistent = Input.RememberMe });

                HttpContext.Session.SetInt32("AccountId", account.AccountId);
                HttpContext.Session.SetString("AccountName", account.AccountName ?? "");
                HttpContext.Session.SetString("Role", roleName);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (roleName == "Staff")
                    return RedirectToPage("/Staff/NewsArticle/Index");
                else
                    return RedirectToPage("/News/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
            return Page();
        }
    }
}
