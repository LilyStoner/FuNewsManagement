using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Services;
using Assignment1_PRN232_FE.Models;

namespace Assignment1_PRN232_FE.Pages.Staff.Tags
{
    public class ArticlesModel : PageModel
    {
        private readonly IApiService _apiService;

        public ArticlesModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public TagModel? Tag { get; set; }
        public List<NewsArticleModel> Articles { get; set; } = new List<NewsArticleModel>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is logged in and is staff
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            if (userRole != "1") // Only Staff can manage tags
            {
                return RedirectToPage("/Access/Denied");
            }

            _apiService.SetAuthToken(token);

            try
            {
                // Get tag details
                var tagResponse = await _apiService.GetByIdAsync<TagModel>("/odata/Tags", id);
                Tag = tagResponse;

                if (Tag == null)
                {
                    TempData["ErrorMessage"] = "Tag not found.";
                    return RedirectToPage("./Index");
                }

                // Get articles using this tag via TagsFunctions controller
                var articlesResponse = await _apiService.GetAsync<NewsArticleModel>($"/odata/TagsFunctions/ArticlesByTag?tagId={id}");
                Articles = articlesResponse?.OrderByDescending(a => a.CreatedDate).ToList() ?? new List<NewsArticleModel>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading articles: {ex.Message}";
                Articles = new List<NewsArticleModel>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveTagAsync(int tagId, string articleId)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token) || userRole != "1")
            {
                return RedirectToPage("/Access/Denied");
            }

            _apiService.SetAuthToken(token);

            try
            {
                // Get current article with all its tags
                var articleResponse = await _apiService.GetByIdAsync<NewsArticleModel>($"/odata/NewsArticles", $"'{articleId}'", "?$expand=Tags");
                
                if (articleResponse == null)
                {
                    TempData["ErrorMessage"] = "Article not found.";
                    return RedirectToPage("./Articles", new { id = tagId });
                }

                // Remove the tag from article's tag list
                var updatedTagIds = articleResponse.Tags?
                    .Where(t => t.TagId != tagId)
                    .Select(t => t.TagId)
                    .ToList() ?? new List<int>();

                // Update article with new tag list
                var updateData = new
                {
                    NewsTitle = articleResponse.NewsTitle,
                    Headline = articleResponse.Headline,
                    NewsContent = articleResponse.NewsContent,
                    NewsSource = articleResponse.NewsSource,
                    CategoryId = articleResponse.CategoryId,
                    NewsStatus = articleResponse.NewsStatus,
                    TagIds = updatedTagIds
                };

                var result = await _apiService.PutAsync<NewsArticleModel>("/odata/NewsArticles", $"'{articleId}'", updateData);
                
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Tag removed from article successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to remove tag from article.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToPage("./Articles", new { id = tagId });
        }
    }
}
