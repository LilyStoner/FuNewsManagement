using System.ComponentModel.DataAnnotations;

namespace Assigment1_PRN232_BE.DTOs
{
    public class NewsListDto
    {
        [Key]
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
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
    }
}