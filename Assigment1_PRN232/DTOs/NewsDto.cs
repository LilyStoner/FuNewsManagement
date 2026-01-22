using System.ComponentModel.DataAnnotations;

namespace Assigment1_PRN232_BE.DTOs
{
    public class NewsCreateDto
    {
        public string? NewsArticleId { get; set; }
        
        [Required]
        [StringLength(400)]
        public string? NewsTitle { get; set; }
        
        [Required] 
        [StringLength(150)]
        public string? Headline { get; set; }
        
        [StringLength(4000)]
        public string? NewsContent { get; set; }
        
        [StringLength(400)]
        public string? NewsSource { get; set; }
        
        public short? CategoryId { get; set; }
        
        public bool? NewsStatus { get; set; }
    }

    public class NewsUpdateDto
    {
        [StringLength(400)]
        public string? NewsTitle { get; set; }
        
        [StringLength(150)]
        public string? Headline { get; set; }
        
        [StringLength(4000)]
        public string? NewsContent { get; set; }
        
        [StringLength(400)]
        public string? NewsSource { get; set; }
        
        public short? CategoryId { get; set; }
        
        public bool? NewsStatus { get; set; }
    }
}