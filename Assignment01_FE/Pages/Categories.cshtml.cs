using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

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
    public int Page { get; set; } = 1;
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public List<CategoryDto> Categories { get; set; } = new();
    public List<CategoryDto> PagedCategories { get; set; } = new();
    public int TotalPages { get; set; }

    public string? UserRole { get; set; }
    public bool CanManage => UserRole == "1" || UserRole == "Admin"; // Staff or Admin

    public async Task OnGetAsync()
    {
        // Get user role from session
        UserRole = HttpContext.Session.GetString("auth_role");

        var client = _httpFactory.CreateClient();
        var url = "https://localhost:7215/api/category" + (string.IsNullOrWhiteSpace(Search) ? string.Empty : $"?search={System.Net.WebUtility.UrlEncode(Search)}");
        
        try
        {
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                Categories = await res.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
            }
        }
        catch (Exception)
        {
            Categories = new List<CategoryDto>();
        }

        // paging
        if (Page < 1) Page = 1;
        if (PageSize <= 0) PageSize = 10;
        TotalPages = (int)Math.Ceiling((double)Categories.Count / PageSize);
        if (TotalPages == 0) TotalPages = 1;
        if (Page > TotalPages) Page = TotalPages;
        PagedCategories = Categories.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
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
}
