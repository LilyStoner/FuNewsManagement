using System.ComponentModel.DataAnnotations;

namespace Assignment1_PRN232_FE.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        
        public bool RememberMe { get; set; } = false;
    }

    public class LoginResponseModel
    {
        public string Token { get; set; } = string.Empty;
        public SystemAccountModel Account { get; set; } = new SystemAccountModel();
        public DateTime ExpiresAt { get; set; }
    }

    public class SystemAccountModel
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string RoleName => AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "Admin"
        };
        public int ArticleCount { get; set; }
    }

    public class CategoryModel
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? ParentCategoryName { get; set; }
        public int ArticleCount { get; set; }
    }

    public class NewsArticleModel
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public List<TagModel> Tags { get; set; } = new List<TagModel>();
    }

    public class TagModel
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public int ArticleCount { get; set; }
    }

    public class ReportModel
    {
        public object? Data { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    // Pagination Models
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => HasPreviousPage ? CurrentPage - 1 : 1;
        public int NextPage => HasNextPage ? CurrentPage + 1 : TotalPages;
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 9; // 3x3 grid
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

        public List<int> GetPageNumbers()
        {
            var pages = new List<int>();
            var start = Math.Max(1, CurrentPage - 2);
            var end = Math.Min(TotalPages, CurrentPage + 2);

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            return pages;
        }
    }

    // Dashboard Models
    public class DashboardStatisticsModel
    {
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public int TotalCategories { get; set; }
        public int ActiveCategories { get; set; }
        public int TotalAccounts { get; set; }
        public int StaffAccounts { get; set; }
        public int LecturerAccounts { get; set; }
        public int TotalTags { get; set; }
        public List<RecentArticleModel> RecentArticles { get; set; } = new List<RecentArticleModel>();
        public MonthlyStatsModel? MonthlyStats { get; set; }
    }

    public class RecentArticleModel
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MonthlyStatsModel
    {
        public int Year { get; set; }
        public List<MonthlyStatistic> MonthlyStatistics { get; set; } = new List<MonthlyStatistic>();
        public int YearTotal { get; set; }
        public int YearActive { get; set; }
        public int YearInactive { get; set; }
    }

    public class MonthlyStatistic
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
    }

    // Staff Dashboard Models
    public class StaffDashboardStatisticsModel
    {
        public int MyTotalArticles { get; set; }
        public int MyActiveArticles { get; set; }
        public int MyInactiveArticles { get; set; }
        public List<RecentArticleModel> MyRecentArticles { get; set; } = new List<RecentArticleModel>();
    }

    // Reports Models
    public class ReportDashboardModel
    {
        public int TotalArticles { get; set; }
        public int PublishedArticles { get; set; }
        public int DraftArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAccounts { get; set; }
        public int TotalTags { get; set; }
    }

    public class CategoryReportModel
    {
        public PeriodModel Period { get; set; } = new PeriodModel();
        public List<CategoryStatisticModel> CategoryStatistics { get; set; } = new List<CategoryStatisticModel>();
    }

    public class AuthorReportModel
    {
        public PeriodModel Period { get; set; } = new PeriodModel();
        public List<AuthorStatisticModel> AuthorStatistics { get; set; } = new List<AuthorStatisticModel>();
    }

    public class PeriodModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CategoryStatisticModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public DateTime? LatestArticle { get; set; }
        public double Percentage { get; set; }
    }

    public class AuthorStatisticModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public DateTime? LatestArticle { get; set; }
        public DateTime? FirstArticle { get; set; }
        public int Role => 1; // Default to Staff for display
        public string LastArticleDate => LatestArticle?.ToString("MMM dd, yyyy") ?? "Never";
    }
}