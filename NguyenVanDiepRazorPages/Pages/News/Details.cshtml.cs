using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public DetailsModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public NewsArticle? Article { get; set; }

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Article = _newsService.GetNewsById(id);
            if (Article == null)
            {
                return NotFound();
            }

            if ((Article.NewsStatus != true) && (!User.IsInRole("Admin") && !User.IsInRole("Staff")))
            {
                return NotFound();
            }

            return Page();
        }
    }
}
