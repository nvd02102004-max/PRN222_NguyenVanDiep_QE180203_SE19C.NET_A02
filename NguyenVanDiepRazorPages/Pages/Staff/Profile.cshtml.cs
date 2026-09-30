using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Staff
{
    public class ProfileModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public ProfileModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public short AccountId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        public string AccountName { get; set; } = string.Empty;

        public string AccountEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [StringLength(70, MinimumLength = 1, ErrorMessage = "Mật khẩu mới tối thiểu 1 ký tự")]
        public string? NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmPassword { get; set; }

        public IActionResult OnGet()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!short.TryParse(userIdStr, out short id))
            {
                return RedirectToPage("/Login");
            }

            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }

            AccountId = account.AccountId;
            AccountName = account.AccountName ?? "";
            AccountEmail = account.AccountEmail ?? "";
            RoleName = account.AccountRole == 1 ? "Staff" : (account.AccountRole == 2 ? "Lecturer" : "User");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!short.TryParse(userIdStr, out short id) || id != AccountId)
            {
                return Forbid();
            }

            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }

            AccountEmail = account.AccountEmail ?? "";
            RoleName = account.AccountRole == 1 ? "Staff" : "Lecturer";

            if (string.IsNullOrWhiteSpace(AccountName))
            {
                ModelState.AddModelError("AccountName", "Tên không được để trống.");
                return Page();
            }

            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (string.IsNullOrEmpty(CurrentPassword) || CurrentPassword != account.AccountPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                    return Page();
                }

                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Xác nhận mật khẩu không khớp.");
                    return Page();
                }

                account.AccountPassword = NewPassword;
            }

            account.AccountName = AccountName;
            _accountService.UpdateAccount(account);

            HttpContext.Session.SetString("AccountName", account.AccountName);
            TempData["SuccessMessage"] = "Cập nhật hồ sơ cá nhân thành công!";
            return RedirectToPage();
        }
    }
}
