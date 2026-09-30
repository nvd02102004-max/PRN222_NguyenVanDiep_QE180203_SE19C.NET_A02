using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public IndexModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        public List<SystemAccount> Accounts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public void OnGet()
        {
            Accounts = _accountService.SearchAccounts(Search);
        }

        public IActionResult OnGetGetAccount(short id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound();

            return new JsonResult(new
            {
                accountId = account.AccountId,
                accountName = account.AccountName,
                accountEmail = account.AccountEmail,
                accountRole = account.AccountRole,
                accountPassword = account.AccountPassword
            });
        }

        public IActionResult OnPostCreate([FromForm] SystemAccount account)
        {
            if (string.IsNullOrWhiteSpace(account.AccountName) ||
                string.IsNullOrWhiteSpace(account.AccountEmail) ||
                string.IsNullOrWhiteSpace(account.AccountPassword) ||
                !account.AccountRole.HasValue)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            var existing = _accountService.GetAccountByEmail(account.AccountEmail);
            if (existing != null)
            {
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return RedirectToPage();
            }

            if (account.AccountId <= 0)
            {
                account.AccountId = _accountService.GetNextAccountId();
            }

            _accountService.CreateAccount(account);
            TempData["SuccessMessage"] = "Account created successfully.";
            return RedirectToPage();
        }

        public IActionResult OnPostEdit([FromForm] SystemAccount account)
        {
            if (account.AccountId <= 0 || string.IsNullOrWhiteSpace(account.AccountName) || string.IsNullOrWhiteSpace(account.AccountEmail))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            var existingEmail = _accountService.GetAccountByEmail(account.AccountEmail);
            if (existingEmail != null && existingEmail.AccountId != account.AccountId)
            {
                TempData["ErrorMessage"] = "Email address is already in use by another account.";
                return RedirectToPage();
            }

            _accountService.UpdateAccount(account);
            TempData["SuccessMessage"] = "Account updated successfully.";
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(short id)
        {
            var success = _accountService.DeleteAccount(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Account deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to delete account.";
            }
            return RedirectToPage();
        }
    }
}
