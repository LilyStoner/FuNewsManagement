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
}