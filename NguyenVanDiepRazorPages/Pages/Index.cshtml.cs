using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenVanDiepRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToPage("/Admin/Accounts/Index");
                if (User.IsInRole("Staff"))
                    return RedirectToPage("/Staff/NewsArticle/Index");
                return RedirectToPage("/News/Index");
            }

            return RedirectToPage("/Login");
        }
    }
}
