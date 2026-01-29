using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1_PRN232_FE.Models;
using Assignment1_PRN232_FE.Services;
using System.Text.Json;

namespace Assignment1_PRN232_FE.Pages.Admin
{
    public class AuditModel : PageModel
    {
        private readonly IApiService _apiService;

        public AuditModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public AuditReportModel AuditReport { get; set; } = new AuditReportModel();
        public List<SystemAccountModel> Editors { get; set; } = new List<SystemAccountModel>();
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public short? UpdatedById { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Access denied. Only Admins can view audit logs.";
                return RedirectToPage("/Index");
            }

            _apiService.SetAuthToken(token);

            try
            {
                // Load all accounts for filter dropdown
                var accountsResponse = await _apiService.GetAsync<SystemAccountModel>("/odata/SystemAccounts?$filter=accountRole eq 1 or accountRole eq 2");
                Editors = accountsResponse ?? new List<SystemAccountModel>();

                // Build query parameters
                var queryParams = new List<string>();
                
                if (StartDate.HasValue)
                    queryParams.Add($"startDate={StartDate:yyyy-MM-dd}");
                
                if (EndDate.HasValue)
                    queryParams.Add($"endDate={EndDate:yyyy-MM-dd}");
                
                if (UpdatedById.HasValue)
                    queryParams.Add($"updatedById={UpdatedById}");
                
                queryParams.Add($"page={CurrentPage}");
                queryParams.Add($"pageSize={PageSize}");

                var endpoint = $"/api/Reports/Audit?{string.Join("&", queryParams)}";
                
                // Get audit data
                var response = await _apiService.GetByIdAsync<JsonElement>(endpoint);
                
                if (response.ValueKind != JsonValueKind.Undefined)
                {
                    var items = response.GetProperty("items").Deserialize<List<AuditLogModel>>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    AuditReport = new AuditReportModel
                    {
                        Items = items ?? new List<AuditLogModel>(),
                        TotalItems = response.GetProperty("totalItems").GetInt32(),
                        TotalPages = response.GetProperty("totalPages").GetInt32(),
                        CurrentPage = response.GetProperty("currentPage").GetInt32(),
                        PageSize = response.GetProperty("pageSize").GetInt32()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading audit log: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading audit log. Please try again.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(token) || userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage();
            }

            _apiService.SetAuthToken(token);

            try
            {
                // Get all audit data (without pagination) for export
                var queryParams = new List<string>();
                
                if (StartDate.HasValue)
                    queryParams.Add($"startDate={StartDate:yyyy-MM-dd}");
                
                if (EndDate.HasValue)
                    queryParams.Add($"endDate={EndDate:yyyy-MM-dd}");
                
                if (UpdatedById.HasValue)
                    queryParams.Add($"updatedById={UpdatedById}");
                
                queryParams.Add("page=1");
                queryParams.Add("pageSize=10000"); // Get all records

                var endpoint = $"/api/Reports/Audit?{string.Join("&", queryParams)}";
                var response = await _apiService.GetByIdAsync<JsonElement>(endpoint);
                
                if (response.ValueKind != JsonValueKind.Undefined)
                {
                    var items = response.GetProperty("items").Deserialize<List<AuditLogModel>>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Export to Excel (you'll need to implement ExcelExportService)
                    // var excelFile = ExcelExportService.ExportAuditLog(items);
                    // return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    //     $"AuditLog_{DateTime.Now:yyyyMMdd}.xlsx");
                    
                    TempData["InfoMessage"] = "Excel export feature will be implemented soon.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting audit log: {ex.Message}");
                TempData["ErrorMessage"] = "Error exporting audit log.";
            }

            return RedirectToPage(new { StartDate, EndDate, UpdatedById, CurrentPage, PageSize });
        }

        public string GetPageUrl(int page)
        {
            var queryParams = new List<string>();
            
            if (StartDate.HasValue)
                queryParams.Add($"startDate={StartDate:yyyy-MM-dd}");
            
            if (EndDate.HasValue)
                queryParams.Add($"endDate={EndDate:yyyy-MM-dd}");
            
            if (UpdatedById.HasValue)
                queryParams.Add($"updatedById={UpdatedById}");
            
            if (PageSize != 20)
                queryParams.Add($"pageSize={PageSize}");
            
            queryParams.Add($"currentPage={page}");
            
            return $"/Admin/Audit" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
        }
    }
}
