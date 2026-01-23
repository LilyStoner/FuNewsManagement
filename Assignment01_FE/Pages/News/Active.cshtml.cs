using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Models;
using Assignment1_PRN232_FE.Services;

namespace Assignment1_PRN232_FE.Pages.News
{
    public class ActiveModel : PageModel
    {
        private readonly IApiService _apiService;

        public ActiveModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<NewsArticleModel> ActiveArticles { get; set; } = new List<NewsArticleModel>();
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public string SearchTerm { get; set; } = string.Empty;
        public short? SelectedCategoryId { get; set; }

        public async Task<IActionResult> OnGetAsync(string? search, short? categoryId)
        {
            SearchTerm = search ?? "";
            SelectedCategoryId = categoryId;

            try
            {
                // Get active articles
                var articles = await _apiService.GetAsync<NewsArticleModel>("/odata/NewsArticles/GetActive");
                ActiveArticles = articles ?? new List<NewsArticleModel>();

                // Filter by search term if provided
                if (!string.IsNullOrEmpty(search))
                {
                    ActiveArticles = ActiveArticles.Where(a => 
                        a.NewsTitle?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                        a.NewsContent?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }

                // Filter by category if provided
                if (categoryId.HasValue)
                {
                    ActiveArticles = ActiveArticles.Where(a => a.CategoryId == categoryId).ToList();
                }

                // Get categories for filter dropdown
                var categoriesResponse = await _apiService.GetAsync<CategoryModel>("/odata/Categories/GetActive");
                Categories = categoriesResponse ?? new List<CategoryModel>();
            }
            catch (Exception)
            {
                // Handle error silently
                ActiveArticles = new List<NewsArticleModel>();
                Categories = new List<CategoryModel>();
            }

            return Page();
        }
    }
}