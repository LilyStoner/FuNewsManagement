using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Assigment1_PRN232_BE.Services;

namespace Assigment1_PRN232_BE.Controllers
{
    [Route("odata/[controller]")]
    [Authorize(Policy = "StaffOnly")]
    [ApiController]
    public class NewsArticlesFunctionsController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesFunctionsController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [HttpPost("Duplicate")]
        public async Task<IActionResult> DuplicateArticle([FromBody] DuplicateArticleRequest request)
        {
            try
            {
                // Get current user ID from claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!short.TryParse(userIdClaim, out short userId))
                {
                    return Unauthorized(new { message = "Invalid user identification" });
                }

                var duplicatedArticle = await _newsArticleService.DuplicateArticleAsync(request.ArticleId, userId);
                
                return Ok(new { 
                    message = "Article duplicated successfully",
                    articleId = duplicatedArticle.NewsArticleId,
                    article = duplicatedArticle
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while duplicating the article", error = ex.Message });
            }
        }

        [HttpGet("Search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchArticles(
            [FromQuery] string? title = null,
            [FromQuery] string? authorName = null,
            [FromQuery] string? categoryName = null,
            [FromQuery] bool? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var articles = await _newsArticleService.SearchNewsArticlesAsync(
                    title, authorName, categoryName, status, startDate, endDate);
                
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching articles", error = ex.Message });
            }
        }
    }

    public class DuplicateArticleRequest
    {
        public string ArticleId { get; set; } = string.Empty;
    }
}