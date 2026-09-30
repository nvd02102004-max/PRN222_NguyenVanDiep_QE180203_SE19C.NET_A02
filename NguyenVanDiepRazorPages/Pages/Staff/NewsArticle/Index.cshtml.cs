using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NguyenVanDiepRazorPages.Hubs;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Staff.NewsArticle
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IHubContext<NewsHub> _hubContext;

        public IndexModel(
            INewsArticleService newsService,
            ICategoryService categoryService,
            ITagService tagService,
            IHubContext<NewsHub> hubContext)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _hubContext = hubContext;
        }

        public List<BusinessObjects.NewsArticle> Articles { get; set; } = new();
        public List<BusinessObjects.Category> Categories { get; set; } = new();
        public List<BusinessObjects.Tag> Tags { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public short? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? Status { get; set; }

        public void OnGet()
        {
            LoadData();
        }

        private void LoadData()
        {
            Articles = _newsService.SearchNews(Search, CategoryId, Status);
            Categories = _categoryService.GetAllCategories();
            Tags = _tagService.GetAllTags();
        }

        public IActionResult OnGetGetArticle(string id)
        {
            var article = _newsService.GetNewsById(id);
            if (article == null)
                return NotFound();

            var tagIds = article.NewsTags.Select(nt => nt.TagId).ToList();

            return new JsonResult(new
            {
                newsArticleId = article.NewsArticleId,
                newsTitle = article.NewsTitle,
                headline = article.Headline,
                newsContent = article.NewsContent,
                newsSource = article.NewsSource,
                categoryId = article.CategoryId,
                newsStatus = article.NewsStatus,
                selectedTagIds = tagIds
            });
        }

        public async Task<IActionResult> OnPostCreateAsync(
            [FromForm] BusinessObjects.NewsArticle article,
            [FromForm] List<int>? selectedTags)
        {
            if (string.IsNullOrWhiteSpace(article.NewsArticleId) ||
                string.IsNullOrWhiteSpace(article.NewsTitle) ||
                string.IsNullOrWhiteSpace(article.Headline) ||
                !article.CategoryId.HasValue)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields (Article ID, Title, Headline, Category).";
                return RedirectToPage();
            }

            var existing = _newsService.GetNewsById(article.NewsArticleId.Trim());
            if (existing != null)
            {
                TempData["ErrorMessage"] = "An article with this ID already exists. Please choose a different ID.";
                return RedirectToPage();
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (short.TryParse(userIdStr, out short authorId))
            {
                article.CreatedById = authorId;
            }

            article.CreatedDate = DateTime.Now;
            article.ModifiedDate = DateTime.Now;

            try
            {
                _newsService.CreateNews(article, selectedTags);
                TempData["SuccessMessage"] = "News article created successfully!";

                // Real-time broadcast via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "create", article.NewsArticleId, article.NewsTitle);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating article: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(
            [FromForm] BusinessObjects.NewsArticle article,
            [FromForm] List<int>? selectedTags)
        {
            if (string.IsNullOrWhiteSpace(article.NewsArticleId) ||
                string.IsNullOrWhiteSpace(article.NewsTitle) ||
                string.IsNullOrWhiteSpace(article.Headline) ||
                !article.CategoryId.HasValue)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (short.TryParse(userIdStr, out short currentUserId))
            {
                article.UpdatedById = currentUserId;
            }

            article.ModifiedDate = DateTime.Now;

            try
            {
                _newsService.UpdateNews(article, selectedTags);
                TempData["SuccessMessage"] = "News article updated successfully!";

                // Real-time broadcast via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "update", article.NewsArticleId, article.NewsTitle);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating article: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            try
            {
                var article = _newsService.GetNewsById(id);
                var title = article?.NewsTitle ?? id;

                var success = _newsService.DeleteNews(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "News article deleted successfully!";

                    // Real-time broadcast via SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "delete", id, title);
                }
                else
                {
                    TempData["ErrorMessage"] = "Article not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting article: " + ex.Message;
            }

            return RedirectToPage();
        }
    }
}
