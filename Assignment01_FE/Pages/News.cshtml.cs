using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
    public string? Id { get; set; }
    [BindProperty(SupportsGet = true, Name = "new")]
    public bool New { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? From { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? To { get; set; }
    [BindProperty(SupportsGet = true)]
    public bool? Status { get; set; }

    public List<NewsDto> News { get; set; } = new();

    [BindProperty]
    public NewsDto EditModel { get; set; } = new();

    public bool IsEditing { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpFactory.CreateClient();

        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(Search)) query.Add($"search={System.Net.WebUtility.UrlEncode(Search)}");
        if (!string.IsNullOrWhiteSpace(Role)) query.Add($"role={System.Net.WebUtility.UrlEncode(Role)}");
        if (From.HasValue) query.Add($"from={From.Value:yyyy-MM-dd}");
        if (To.HasValue) query.Add($"to={To.Value:yyyy-MM-dd}");
        if (Status.HasValue) query.Add($"status={Status.Value.ToString().ToLower()}");

        var url = "https://localhost:7215/api/news" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);

        var res = await client.GetAsync(url);
        if (res.IsSuccessStatusCode)
        {
            var list = await res.Content.ReadFromJsonAsync<List<NewsDto>>();
            if (list != null) News = list;
        }

        if (!string.IsNullOrEmpty(Id))
        {
            var r2 = await client.GetAsync($"https://localhost:7215/api/news/{Id}");
            if (r2.IsSuccessStatusCode)
            {
                var n = await r2.Content.ReadFromJsonAsync<NewsDto>();
                if (n != null) { EditModel = n; IsEditing = true; }
            }
        }
        else if (New)
        {
            IsEditing = true;
            EditModel = new NewsDto();
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (string.IsNullOrEmpty(EditModel.NewsArticleId))
        {
            EditModel.NewsArticleId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var res = await client.PostAsJsonAsync("https://localhost:7215/api/news", EditModel);
            // ignore response handling for brevity
        }
        else
        {
            var res = await client.PutAsJsonAsync($"https://localhost:7215/api/news/{EditModel.NewsArticleId}", EditModel);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.DeleteAsync($"https://localhost:7215/api/news/{id}");
        return RedirectToPage();
    }

    public class NewsDto
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // helper fields returned from API for display
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
    }
}
