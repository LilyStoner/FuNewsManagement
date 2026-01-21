using Assigment1_PRN232_BE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Assigment1_PRN232_BE.Services;
using System.Linq;

namespace Assigment1_PRN232_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        // Anyone can view published news, supports search, role filter, date range and status
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<NewsListDto>> Get([FromQuery] string? search, [FromQuery] string? role, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] bool? status)
        {
            IEnumerable<NewsArticle> items;
            if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(role) && !from.HasValue && !to.HasValue && !status.HasValue)
            {
                items = await _service.GetAllPublishedAsync();
            }
            else
            {
                items = await _service.SearchPublishedAsync(search, role, from, to, status);
            }

            var dto = items.Select(n => new NewsListDto
            {
                NewsArticleId = n.NewsArticleId,
                NewsTitle = n.NewsTitle,
                Headline = n.Headline,
                CreatedDate = n.CreatedDate,
                NewsContent = n.NewsContent,
                NewsSource = n.NewsSource,
                CategoryId = n.CategoryId,
                NewsStatus = n.NewsStatus,
                CreatedById = n.CreatedById,
                UpdatedById = n.UpdatedById,
                ModifiedDate = n.ModifiedDate,
                AuthorName = n.CreatedBy?.AccountName,
                CategoryName = n.Category?.CategoryName
            }).ToList();

            return dto;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<NewsListDto>> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (item.NewsStatus != true) return Forbid();

            var dto = new NewsListDto
            {
                NewsArticleId = item.NewsArticleId,
                NewsTitle = item.NewsTitle,
                Headline = item.Headline,
                CreatedDate = item.CreatedDate,
                NewsContent = item.NewsContent,
                NewsSource = item.NewsSource,
                CategoryId = item.CategoryId,
                NewsStatus = item.NewsStatus,
                CreatedById = item.CreatedById,
                UpdatedById = item.UpdatedById,
                ModifiedDate = item.ModifiedDate,
                AuthorName = item.CreatedBy?.AccountName,
                CategoryName = item.Category?.CategoryName
            };

            return dto;
        }

        // Create/Update/Delete require Staff (1) or Admin
        [HttpPost]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Post([FromBody] NewsArticle news)
        {
            // For skeleton, using a fixed current user id 1. Replace with actual auth user id.
            await _service.AddAsync(news, 1);
            return CreatedAtAction(nameof(Get), new { id = news.NewsArticleId }, news);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticle news)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            news.NewsArticleId = id;
            await _service.UpdateAsync(news, 1);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }

    public class NewsListDto
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
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
    }
}