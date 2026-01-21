using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
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
    public int? Id { get; set; }
    [BindProperty(SupportsGet = true, Name = "new")]
    public bool New { get; set; }
    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public List<TagDto> Tags { get; set; } = new();
    public List<TagDto> PagedTags { get; set; } = new();
    public int TotalPages { get; set; }

    [BindProperty]
    public TagDto EditModel { get; set; } = new();

    public bool IsEditing { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpFactory.CreateClient();
        var url = "https://localhost:7215/api/tag" + (string.IsNullOrWhiteSpace(Search) ? string.Empty : $"?search={System.Net.WebUtility.UrlEncode(Search)}");
        var res = await client.GetAsync(url);
        if (res.IsSuccessStatusCode)
        {
            Tags = await res.Content.ReadFromJsonAsync<List<TagDto>>() ?? new List<TagDto>();
        }

        // paging
        if (Page < 1) Page = 1;
        if (PageSize <= 0) PageSize = 10;
        TotalPages = (int)Math.Ceiling((double)Tags.Count / PageSize);
        if (TotalPages == 0) TotalPages = 1;
        if (Page > TotalPages) Page = TotalPages;
        PagedTags = Tags.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

        if (Id.HasValue)
        {
            var r2 = await client.GetAsync($"https://localhost:7215/api/tag/{Id.Value}");
            if (r2.IsSuccessStatusCode)
            {
                EditModel = await r2.Content.ReadFromJsonAsync<TagDto>() ?? new TagDto();
                IsEditing = true;
            }
        }
        else if (New)
        {
            IsEditing = true;
            EditModel = new TagDto();
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (EditModel.TagId == 0)
        {
            var req = new CreateTagRequest { TagName = EditModel.TagName, Note = EditModel.Note };
            var res = await client.PostAsJsonAsync("https://localhost:7215/api/tag", req);
            if (res.IsSuccessStatusCode) TempData["Success"] = "Tag created.";
            else TempData["Error"] = await res.Content.ReadAsStringAsync();
        }
        else
        {
            var req = new UpdateTagRequest { TagName = EditModel.TagName, Note = EditModel.Note };
            var res = await client.PutAsJsonAsync($"https://localhost:7215/api/tag/{EditModel.TagId}", req);
            if (res.IsSuccessStatusCode) TempData["Success"] = "Tag updated.";
            else TempData["Error"] = await res.Content.ReadAsStringAsync();
        }

        return RedirectToPage(new { search = Search, page = Page, pageSize = PageSize });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.DeleteAsync($"https://localhost:7215/api/tag/{id}");
        if (res.IsSuccessStatusCode) TempData["Success"] = "Tag deleted.";
        else TempData["Error"] = await res.Content.ReadAsStringAsync();

        return RedirectToPage(new { search = Search, page = Page, pageSize = PageSize });
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public int ArticleCount { get; set; }
    }

    public class CreateTagRequest
    {
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateTagRequest
    {
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }
}
