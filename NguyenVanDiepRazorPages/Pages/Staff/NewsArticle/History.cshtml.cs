using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Staff.NewsArticle
{
    public class HistoryModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public HistoryModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public List<BusinessObjects.NewsArticle> MyArticles { get; set; } = new();

        public IActionResult OnGet()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!short.TryParse(userIdStr, out short authorId))
            {
                return RedirectToPage("/Login");
            }

            MyArticles = _newsService.GetNewsByAuthor(authorId);
            return Page();
        }
    }
}
