using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Services;

namespace Assignment1_PRN232_FE.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IApiService _apiService;

        public DashboardModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public dynamic? DashboardData { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in and is admin
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            if (userRole != "Admin")
            {
                return RedirectToPage("/Staff/Dashboard");
            }

            UserName = HttpContext.Session.GetString("UserName") ?? "Admin";
            UserRole = userRole ?? "Admin";

            // Set auth token
            _apiService.SetAuthToken(token);

            try
            {
                // Get dashboard statistics
                var response = await _apiService.GetAsync<dynamic>("/api/Reports/Dashboard");
                DashboardData = response?.FirstOrDefault();
            }
            catch (Exception)
            {
                // Handle error silently, dashboard will show without data
                DashboardData = null;
            }

            return Page();
        }
    }
}