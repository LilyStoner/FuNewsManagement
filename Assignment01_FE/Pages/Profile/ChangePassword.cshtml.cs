using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Services;
using System.ComponentModel.DataAnnotations;

namespace Assignment1_PRN232_FE.Pages.Profile
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IApiService _apiService;

        public ChangePasswordModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public PasswordChangeModel PasswordChange { get; set; } = new PasswordChangeModel();

        public string UserRole { get; set; } = "";
        public string UserName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            // Only Staff (role = "1") and Lecturer (role = "2") can change password
            if (userRole != "1" && userRole != "2")
            {
                TempData["ErrorMessage"] = "Access denied. Password management is only available for Staff and Lecturer accounts.";
                
                // Redirect Admin to dashboard
                if (userRole == "Admin")
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                
                return RedirectToPage("/Login");
            }

            UserRole = userRole;
            UserName = userName ?? "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            
            // Verify access permission
            if (userRole != "1" && userRole != "2")
            {
                TempData["ErrorMessage"] = "Access denied. Password changes are only allowed for Staff and Lecturer members.";
                return RedirectToPage();
            }

            // Validate password change model
            if (string.IsNullOrEmpty(PasswordChange.CurrentPassword) || 
                string.IsNullOrEmpty(PasswordChange.NewPassword) ||
                string.IsNullOrEmpty(PasswordChange.ConfirmPassword))
            {
                TempData["ErrorMessage"] = "All password fields are required.";
                return RedirectToPage();
            }

            if (PasswordChange.NewPassword != PasswordChange.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirmation do not match.";
                return RedirectToPage();
            }

            if (PasswordChange.NewPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "New password must be at least 6 characters long.";
                return RedirectToPage();
            }

            if (PasswordChange.NewPassword == PasswordChange.CurrentPassword)
            {
                TempData["ErrorMessage"] = "New password must be different from current password.";
                return RedirectToPage();
            }

            var token = HttpContext.Session.GetString("AuthToken");
            _apiService.SetAuthToken(token!);

            try
            {
                var changePasswordData = new
                {
                    CurrentPassword = PasswordChange.CurrentPassword,
                    NewPassword = PasswordChange.NewPassword,
                    ConfirmPassword = PasswordChange.ConfirmPassword
                };

                // Use the SystemAccountsFunctions endpoint for password change
                var result = await _apiService.PostAsync<object>("/odata/SystemAccountsFunctions/ChangePassword", changePasswordData);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Password changed successfully! Please login again with your new password.";
                    
                    // Clear session and redirect to login
                    HttpContext.Session.Clear();
                    return RedirectToPage("/Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to change password. Current password may be incorrect.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while changing password.";
            }

            return RedirectToPage();
        }
    }

    public class PasswordChangeModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(70, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 70 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}