using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Assigment1_PRN232_BE.Services;

namespace Assigment1_PRN232_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly INewsArticleService _newsArticleService;

        public ReportsController(IReportService reportService, INewsArticleService newsArticleService)
        {
            _reportService = reportService;
            _newsArticleService = newsArticleService;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            try
            {
                var statistics = await _reportService.GetDashboardStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics", error = ex.Message });
            }
        }

        [HttpGet("ArticlesByPeriod")]
        public async Task<IActionResult> GetArticleStatisticsByPeriod(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var statistics = await _reportService.GetArticleStatisticsByPeriodAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving article statistics by period", error = ex.Message });
            }
        }

        [HttpGet("ArticlesByCategory")]
        public async Task<IActionResult> GetArticleStatisticsByCategory(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var statistics = await _reportService.GetArticleStatisticsByCategoryAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving article statistics by category", error = ex.Message });
            }
        }

        [HttpGet("ArticlesByAuthor")]
        public async Task<IActionResult> GetArticleStatisticsByAuthor(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var statistics = await _reportService.GetArticleStatisticsByAuthorAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving article statistics by author", error = ex.Message });
            }
        }

        [HttpGet("ArticlesByStatus")]
        public async Task<IActionResult> GetArticleStatisticsByStatus(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var statistics = await _reportService.GetArticleStatisticsByStatusAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving article statistics by status", error = ex.Message });
            }
        }

        [HttpGet("CategoryUsage")]
        public async Task<IActionResult> GetCategoryUsageStatistics()
        {
            try
            {
                var statistics = await _reportService.GetCategoryUsageStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving category usage statistics", error = ex.Message });
            }
        }

        [HttpGet("TagUsage")]
        public async Task<IActionResult> GetTagUsageStatistics()
        {
            try
            {
                var statistics = await _reportService.GetTagUsageStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tag usage statistics", error = ex.Message });
            }
        }

        [HttpGet("MonthlyStats")]
        public async Task<IActionResult> GetMonthlyArticleStats([FromQuery] int year = 0)
        {
            try
            {
                if (year == 0)
                {
                    year = DateTime.Now.Year;
                }

                if (year < 2000 || year > DateTime.Now.Year + 1)
                {
                    return BadRequest(new { message = "Invalid year provided" });
                }

                var statistics = await _reportService.GetMonthlyArticleStatsAsync(year);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving monthly article statistics", error = ex.Message });
            }
        }

        [HttpGet("Audit")]
        public async Task<IActionResult> GetChangeAudit(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] short? updatedById,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Get all articles with UpdatedBy information
                var allArticles = await _newsArticleService.GetAllNewsArticlesAsync();

                // Filter articles that have been modified (have UpdatedById and ModifiedDate)
                var modifiedArticles = allArticles
                    .Where(a => a.UpdatedById.HasValue && a.ModifiedDate.HasValue)
                    .AsQueryable();

                // Apply date filters
                if (startDate.HasValue)
                {
                    modifiedArticles = modifiedArticles.Where(a => a.ModifiedDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    modifiedArticles = modifiedArticles.Where(a => a.ModifiedDate <= endDate.Value.AddDays(1).AddSeconds(-1));
                }

                // Filter by updater
                if (updatedById.HasValue)
                {
                    modifiedArticles = modifiedArticles.Where(a => a.UpdatedById == updatedById.Value);
                }

                // Order by modified date descending
                var orderedArticles = modifiedArticles.OrderByDescending(a => a.ModifiedDate).ToList();

                // Calculate pagination
                var totalItems = orderedArticles.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Get page items
                var pagedArticles = orderedArticles
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new
                    {
                        a.NewsArticleId,
                        a.NewsTitle,
                        a.NewsStatus,
                        a.CreatedDate,
                        a.ModifiedDate,
                        a.CreatedById,
                        CreatedByName = a.CreatedBy?.AccountName,
                        a.UpdatedById,
                        UpdatedByName = a.UpdatedBy?.AccountName,
                        CategoryName = a.Category?.CategoryName,
                        a.CategoryId
                    })
                    .ToList();

                return Ok(new
                {
                    items = pagedArticles,
                    totalItems,
                    totalPages,
                    currentPage = page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving audit log", error = ex.Message });
            }
        }

        [HttpGet("TopAuthors")]
        public async Task<IActionResult> GetTopAuthors([FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new { message = "Limit must be between 1 and 100" });
                }

                var statistics = await _reportService.GetTopAuthorsAsync(limit);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving top authors", error = ex.Message });
            }
        }

        [HttpGet("TopCategories")]
        public async Task<IActionResult> GetTopCategories([FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new { message = "Limit must be between 1 and 100" });
                }

                var statistics = await _reportService.GetTopCategoriesAsync(limit);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving top categories", error = ex.Message });
            }
        }
    }
}