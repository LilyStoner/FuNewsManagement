using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Assignment01_FE.Pages;

public class CategoriesModel : PageModel
{
    private readonly IHttpClientFactory _httpFactory;

    public CategoriesModel(IHttpClientFactory httpFactory)
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

    public List<CategoryDto> Categories { get; set; } = new();
    public List<CategoryDto> PagedCategories { get; set; } = new();
    public int TotalPages { get; set; }

    [BindProperty]
    public CategoryDto EditModel { get; set; } = new();

    public bool IsEditing { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpFactory.CreateClient();
        var url = "https://localhost:7215/api/category" + (string.IsNullOrWhiteSpace(Search) ? string.Empty : $"?search={System.Net.WebUtility.UrlEncode(Search)}");
        var res = await client.GetAsync(url);
        if (res.IsSuccessStatusCode)
        {
            Categories = await res.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
        }

        // paging
        if (Page < 1) Page = 1;
        if (PageSize <= 0) PageSize = 10;
        TotalPages = (int)Math.Ceiling((double)Categories.Count / PageSize);
        if (TotalPages == 0) TotalPages = 1;
        if (Page > TotalPages) Page = TotalPages;
        PagedCategories = Categories.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

        if (Id.HasValue)
        {
            var r2 = await client.GetAsync($"https://localhost:7215/api/category/{Id.Value}");
            if (r2.IsSuccessStatusCode)
            {
                EditModel = await r2.Content.ReadFromJsonAsync<CategoryDto>() ?? new CategoryDto();
                IsEditing = true;
            }
        }
        else if (New)
        {
            IsEditing = true;
            EditModel = new CategoryDto();
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (EditModel.CategoryId == 0)
        {
            var req = new CreateCategoryRequest { CategoryName = EditModel.CategoryName, CategoryDesciption = EditModel.CategoryDesciption, ParentCategoryId = EditModel.ParentCategoryId, IsActive = EditModel.IsActive };
            var res = await client.PostAsJsonAsync("https://localhost:7215/api/category", req);
            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category created.";
            }
            else
            {
                var text = await res.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrWhiteSpace(text) ? "Create failed" : text;
            }
        }
        else
        {
            var req = new UpdateCategoryRequest { CategoryName = EditModel.CategoryName, CategoryDesciption = EditModel.CategoryDesciption, ParentCategoryId = EditModel.ParentCategoryId, IsActive = EditModel.IsActive };
            var res = await client.PutAsJsonAsync($"https://localhost:7215/api/category/{EditModel.CategoryId}", req);
            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category updated.";
            }
            else
            {
                var text = await res.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrWhiteSpace(text) ? "Update failed" : text;
            }
        }

        return RedirectToPage(new { search = Search, page = Page, pageSize = PageSize });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var token = HttpContext.Session.GetString("auth_token");
        var client = _httpFactory.CreateClient();
        if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.DeleteAsync($"https://localhost:7215/api/category/{id}");
        if (res.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category deleted.";
        }
        else
        {
            var text = await res.Content.ReadAsStringAsync();
            TempData["Error"] = string.IsNullOrWhiteSpace(text) ? "Delete failed" : text;
        }

        return RedirectToPage(new { search = Search, page = Page, pageSize = PageSize });
    }

    public class CategoryDto
    {
        public short CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int ArticleCount { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
