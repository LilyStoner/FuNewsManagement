using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Services;

namespace Assignment1_PRN232_FE.Pages.Staff
{
    public class DashboardModel : PageModel
    {
        private readonly IApiService _apiService;

        public DashboardModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            UserName = HttpContext.Session.GetString("UserName") ?? "User";
            UserRole = userRole == "1" ? "Staff" : userRole == "2" ? "Lecturer" : "Admin";
            UserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Set auth token
            _apiService.SetAuthToken(token);

            return Page();
        }
    }
}