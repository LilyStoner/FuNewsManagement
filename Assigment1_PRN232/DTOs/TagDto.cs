namespace Assigment1_PRN232_BE.DTOs
{
    public class TagDto
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public int ArticleCount { get; set; }
    }

    public class CreateTagRequest
    {
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateTagRequest
    {
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

}
