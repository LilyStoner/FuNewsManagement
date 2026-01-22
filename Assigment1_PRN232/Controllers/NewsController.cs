using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Assigment1_PRN232_BE.Services;
using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace Assigment1_PRN232_BE.Controllers
{
    // API Controller for News CRUD
    [ApiController]
    [Route("api/news")]
    public class NewsApiController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsApiController(INewsService service)
        {
            _service = service;
        }

        private short GetCurrentUserId()
        {
            var idClaim = User.FindFirst("id")?.Value;
            return short.TryParse(idClaim, out var id) ? id : (short)1;
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

        [HttpPost]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Post([FromBody] NewsCreateDto req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var news = new NewsArticle
            {
                NewsArticleId = req.NewsArticleId ?? Guid.NewGuid().ToString("N")[..20],
                NewsTitle = req.NewsTitle,
                Headline = req.Headline,
                NewsContent = req.NewsContent,
                NewsSource = req.NewsSource,
                CategoryId = req.CategoryId,
                NewsStatus = req.NewsStatus
            };

            try
            {
                var currentUserId = GetCurrentUserId();
                await _service.AddAsync(news, currentUserId);
                return CreatedAtAction(nameof(Get), new { id = news.NewsArticleId }, news);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsUpdateDto req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null) return NotFound();

                if (req.NewsTitle != null) existing.NewsTitle = req.NewsTitle;
                if (req.Headline != null) existing.Headline = req.Headline;
                if (req.NewsContent != null) existing.NewsContent = req.NewsContent;
                if (req.NewsSource != null) existing.NewsSource = req.NewsSource;
                if (req.CategoryId.HasValue) existing.CategoryId = req.CategoryId;
                if (req.NewsStatus.HasValue) existing.NewsStatus = req.NewsStatus;

                var currentUserId = GetCurrentUserId();
                await _service.UpdateAsync(existing, currentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null) return NotFound();
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // OData Controller for News querying
    public class NewsController : ODataController
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        // Anyone can view published news, OData-enabled
        [HttpGet]
        [AllowAnonymous]
        [EnableQuery(PageSize = 10)]
        public IActionResult Get()
        {
            var q = _service.GetPublishedQueryable();
            return Ok(q);
        }
    }
}