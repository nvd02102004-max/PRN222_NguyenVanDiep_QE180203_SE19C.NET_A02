using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Admin
{
    public class ReportModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public ReportModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public List<NewsArticle> Articles { get; set; } = new();
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public Dictionary<string, int> CategoryStats { get; set; } = new();

        public void OnGet()
        {
            Articles = _newsService.GetReport(StartDate, EndDate);
            TotalArticles = Articles.Count;
            ActiveArticles = Articles.Count(a => a.NewsStatus == true);
            InactiveArticles = Articles.Count(a => a.NewsStatus != true);

            var categories = _categoryService.GetAllCategories();
            foreach (var cat in categories)
            {
                int count = Articles.Count(a => a.CategoryId == cat.CategoryId);
                if (count > 0)
                {
                    CategoryStats[cat.CategoryName] = count;
                }
            }
        }
    }
}
