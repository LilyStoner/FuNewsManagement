using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Services;

namespace Assignment1_PRN232_FE.Pages.Admin
{
    public class ReportsModel : PageModel
    {
        private readonly IApiService _apiService;

        public ReportsModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public dynamic? DashboardStats { get; set; }
        public List<dynamic> CategoryStats { get; set; } = new List<dynamic>();
        public List<dynamic> AuthorStats { get; set; } = new List<dynamic>();
        public List<dynamic> MonthlyStats { get; set; } = new List<dynamic>();

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "dashboard";

        public async Task<IActionResult> OnGetAsync()
        {
            // Check authentication and authorization
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token) || userRole != "Admin")
            {
                return RedirectToPage("/Login");
            }

            _apiService.SetAuthToken(token);

            // Set default date range if not provided (last 30 days)
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                EndDate = DateTime.Today;
                StartDate = DateTime.Today.AddDays(-30);
            }

            try
            {
                await LoadReportDataAsync();
            }
            catch (Exception)
            {
                // Handle errors gracefully
                DashboardStats = null;
                CategoryStats = new List<dynamic>();
                AuthorStats = new List<dynamic>();
                MonthlyStats = new List<dynamic>();
            }

            return Page();
        }

        private async Task LoadReportDataAsync()
        {
            switch (ReportType.ToLower())
            {
                case "dashboard":
                    await LoadDashboardStatsAsync();
                    break;
                case "category":
                    await LoadCategoryStatsAsync();
                    break;
                case "author":
                    await LoadAuthorStatsAsync();
                    break;
                case "monthly":
                    await LoadMonthlyStatsAsync();
                    break;
                default:
                    await LoadDashboardStatsAsync();
                    break;
            }
        }

        private async Task LoadDashboardStatsAsync()
        {
            try
            {
                var dashboardResponse = await _apiService.GetAsync<dynamic>("/api/Reports/Dashboard");
                DashboardStats = dashboardResponse?.FirstOrDefault();
            }
            catch
            {
                DashboardStats = new
                {
                    TotalArticles = 0,
                    PublishedArticles = 0,
                    DraftArticles = 0,
                    TotalCategories = 0,
                    TotalAccounts = 0,
                    TotalTags = 0
                };
            }
        }

        private async Task LoadCategoryStatsAsync()
        {
            try
            {
                var categoryResponse = await _apiService.GetAsync<dynamic>($"/api/Reports/ArticlesByCategory?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}");
                CategoryStats = categoryResponse ?? new List<dynamic>();
            }
            catch
            {
                CategoryStats = new List<dynamic>();
            }
        }

        private async Task LoadAuthorStatsAsync()
        {
            try
            {
                var authorResponse = await _apiService.GetAsync<dynamic>($"/api/Reports/ArticlesByAuthor?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}");
                AuthorStats = authorResponse ?? new List<dynamic>();
            }
            catch
            {
                AuthorStats = new List<dynamic>();
            }
        }

        private async Task LoadMonthlyStatsAsync()
        {
            try
            {
                var year = EndDate?.Year ?? DateTime.Now.Year;
                var monthlyResponse = await _apiService.GetAsync<dynamic>($"/api/Reports/MonthlyStats?year={year}");
                MonthlyStats = monthlyResponse ?? new List<dynamic>();
            }
            catch
            {
                MonthlyStats = new List<dynamic>();
            }
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            // Export functionality would be implemented here
            TempData["InfoMessage"] = "Export functionality will be implemented in a future update.";
            return RedirectToPage();
        }
    }
}