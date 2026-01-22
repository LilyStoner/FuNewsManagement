namespace Assigment1_PRN232_BE.DTOs
{

    public class CategoryDto
    {
        public short CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int ArticleCount { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

}
