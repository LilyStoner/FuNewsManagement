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

    public async Task OnGetAsync()
    {
        var client = _httpFactory.CreateClient();

        // build OData query
        int skip = (Page - 1) * PageSize;
        var q = new List<string>();
        q.Add("$top=" + PageSize);
        q.Add("$skip=" + skip);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            // encode search value for URL and include in single quotes for OData contains
            var v = System.Uri.EscapeDataString(Search);
            q.Add("$filter=contains(NewsTitle,'" + v + "') or contains(Headline,'" + v + "') or contains(NewsContent,'" + v + "')");
        }
        q.Add("$orderby=CreatedDate desc");

        var url = "https://localhost:7215/odata/News" + (q.Count > 0 ? "?" + string.Join("&", q) : string.Empty);

        var res = await client.GetAsync(url);
        if (!res.IsSuccessStatusCode) return;

        var json = await res.Content.ReadAsStringAsync();
        // try parse as OData JSON (value array) or fallback to direct list
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("value", out var value))
            {
                News = JsonSerializer.Deserialize<List<NewsDto>>(value.GetRawText()) ?? new List<NewsDto>();
            }
            else
            {
                News = JsonSerializer.Deserialize<List<NewsDto>>(json) ?? new List<NewsDto>();
            }

            if (doc.RootElement.TryGetProperty("@odata.nextLink", out var next))
            {
                NextLink = next.GetString();
            }
            else if (doc.RootElement.TryGetProperty("odata.nextLink", out var next2))
            {
                NextLink = next2.GetString();
            }
        }
        catch
        {
            // fallback
            News = JsonSerializer.Deserialize<List<NewsDto>>(json) ?? new List<NewsDto>();
        }
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
