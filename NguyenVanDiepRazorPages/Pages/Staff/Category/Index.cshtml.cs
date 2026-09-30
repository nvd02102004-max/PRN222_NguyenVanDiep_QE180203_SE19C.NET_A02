using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NguyenVanDiepRazorPages.Pages.Staff.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<BusinessObjects.Category> Categories { get; set; } = new();
        public List<BusinessObjects.Category> AllCategories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public void OnGet()
        {
            Categories = _categoryService.SearchCategories(Search);
            AllCategories = _categoryService.GetAllCategories();
        }

        public IActionResult OnGetGetCategory(short id)
        {
            var cat = _categoryService.GetCategoryById(id);
            if (cat == null)
                return NotFound();

            return new JsonResult(new
            {
                categoryId = cat.CategoryId,
                categoryName = cat.CategoryName,
                categoryDesciption = cat.CategoryDesciption,
                parentCategoryId = cat.ParentCategoryId,
                isActive = cat.IsActive
            });
        }

        public IActionResult OnPostCreate([FromForm] BusinessObjects.Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName) || string.IsNullOrWhiteSpace(category.CategoryDesciption))
            {
                TempData["ErrorMessage"] = "Category Name and Description are required.";
                return RedirectToPage();
            }

            try
            {
                _categoryService.CreateCategory(category);
                TempData["SuccessMessage"] = "Category created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating category: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit([FromForm] BusinessObjects.Category category)
        {
            if (category.CategoryId <= 0 || string.IsNullOrWhiteSpace(category.CategoryName) || string.IsNullOrWhiteSpace(category.CategoryDesciption))
            {
                TempData["ErrorMessage"] = "Invalid data. Please fill in all required fields.";
                return RedirectToPage();
            }

            try
            {
                _categoryService.UpdateCategory(category);
                TempData["SuccessMessage"] = "Category updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating category: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(short id)
        {
            try
            {
                if (_categoryService.HasNewsArticles(id))
                {
                    TempData["ErrorMessage"] = "Cannot delete this category because it is already used in one or more news articles.";
                    return RedirectToPage();
                }

                var success = _categoryService.DeleteCategory(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Category deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}
