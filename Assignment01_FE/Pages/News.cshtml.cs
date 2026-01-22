using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Assignment01_FE.Pages;

public class NewsModel : PageModel
{
    private readonly IHttpClientFactory _httpFactory;

    public NewsModel(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }
    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public List<NewsDto> News { get; set; } = new();
    public string? NextLink { get; set; }
    public int TotalPages { get; set; } = 1;
    public string? UserRole { get; set; }
    public bool CanManage => UserRole == "1" || UserRole == "Admin"; // Staff or Admin

    public async Task OnGetAsync()
    {
        // Get user role from session
        UserRole = HttpContext.Session.GetString("auth_role");

        var client = _httpFactory.CreateClient();

        // build OData query
        int skip = (Page - 1) * PageSize;
        var q = new List<string>();
        q.Add("$top=" + PageSize);
        q.Add("$skip=" + skip);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            // encode search value for URL and include in single quotes for OData contains
            var searchEncoded = System.Uri.EscapeDataString(Search);
            q.Add($"$filter=contains(NewsTitle,'{searchEncoded}') or contains(Headline,'{searchEncoded}') or contains(NewsContent,'{searchEncoded}')");
        }
        q.Add("$orderby=CreatedDate desc");

        var url = "https://localhost:7215/odata/News" + (q.Count > 0 ? "?" + string.Join("&", q) : string.Empty);

        var res = await client.GetAsync(url);
        if (!res.IsSuccessStatusCode) 
        {
            // Log error or show message
            return;
        }

        var json = await res.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("value", out var value))
            {
                // Map from NewsArticle (OData entity) to NewsDto for display
                var articles = JsonSerializer.Deserialize<List<NewsArticleOData>>(value.GetRawText()) ?? new List<NewsArticleOData>();
                News = articles.Select(a => new NewsDto
                {
                    NewsArticleId = a.NewsArticleId,
                    NewsTitle = a.NewsTitle,
                    Headline = a.Headline,
                    CreatedDate = a.CreatedDate,
                    NewsContent = a.NewsContent,
                    NewsSource = a.NewsSource,
                    CategoryId = a.CategoryId,
                    NewsStatus = a.NewsStatus,
                    CreatedById = a.CreatedById,
                    UpdatedById = a.UpdatedById,
                    ModifiedDate = a.ModifiedDate,
                    AuthorName = a.CreatedBy?.AccountName,
                    CategoryName = a.Category?.CategoryName
                }).ToList();
            }

            if (doc.RootElement.TryGetProperty("@odata.nextLink", out var next))
            {
                NextLink = next.GetString();
            }
            else if (doc.RootElement.TryGetProperty("odata.nextLink", out var next2))
            {
                NextLink = next2.GetString();
            }

            // Calculate total pages (rough estimate since OData doesn't provide total count by default)
            TotalPages = Math.Max(1, Page + (string.IsNullOrEmpty(NextLink) ? 0 : 1));
        }
        catch (Exception ex)
        {
            // Log error
            News = new List<NewsDto>();
        }
    }

    // OData entity structure for parsing
    public class NewsArticleOData
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public CreatedByOData? CreatedBy { get; set; }
        public CategoryOData? Category { get; set; }
    }

    public class CreatedByOData
    {
        public string? AccountName { get; set; }
    }

    public class CategoryOData
    {
        public string? CategoryName { get; set; }
    }

    public class NewsDto
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
    }
}
