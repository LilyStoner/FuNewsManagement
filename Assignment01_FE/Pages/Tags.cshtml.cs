using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Assignment01_FE.Pages;

public class TagsModel : PageModel
{
    private readonly IHttpClientFactory _httpFactory;

    public TagsModel(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public List<TagDto> Tags { get; set; } = new();
    public List<TagDto> PagedTags { get; set; } = new();
    public int TotalPages { get; set; }

    public string? UserRole { get; set; }
    public bool CanManage => UserRole == "1" || UserRole == "Admin"; // Staff or Admin

    public async Task OnGetAsync()
    {
        // Get user role from session
        UserRole = HttpContext.Session.GetString("auth_role");

        var client = _httpFactory.CreateClient();
        var url = "https://localhost:7215/api/tag" + (string.IsNullOrWhiteSpace(Search) ? string.Empty : $"?search={System.Net.WebUtility.UrlEncode(Search)}");
        
        try
        {
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                Tags = await res.Content.ReadFromJsonAsync<List<TagDto>>() ?? new List<TagDto>();
            }
        }
        catch (Exception)
        {
            Tags = new List<TagDto>();
        }

        // paging
        if (Page < 1) Page = 1;
        if (PageSize <= 0) PageSize = 10;
        TotalPages = (int)Math.Ceiling((double)Tags.Count / PageSize);
        if (TotalPages == 0) TotalPages = 1;
        if (Page > TotalPages) Page = TotalPages;
        PagedTags = Tags.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public int ArticleCount { get; set; }
    }
}
