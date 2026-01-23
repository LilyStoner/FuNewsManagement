using Microsoft.EntityFrameworkCore;
using Assigment1_PRN232_BE.DataAccess;
using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> GetArticleStatisticsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var articles = await _unitOfWork.NewsArticleRepository.Query()
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            var statistics = new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                TotalArticles = articles.Count,
                ActiveArticles = articles.Count(a => a.NewsStatus == true),
                InactiveArticles = articles.Count(a => a.NewsStatus == false),
                Articles = articles.Select(a => new
                {
                    a.NewsArticleId,
                    a.NewsTitle,
                    a.CreatedDate,
                    CategoryName = a.Category?.CategoryName,
                    AuthorName = a.CreatedBy?.AccountName,
                    Status = a.NewsStatus == true ? "Active" : "Inactive"
                }).ToList()
            };

            return statistics;
        }

        public async Task<object> GetArticleStatisticsByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _unitOfWork.NewsArticleRepository.Query()
                .Include(n => n.Category);

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate);

            var articles = await query.ToListAsync();

            var categoryStats = articles
                .GroupBy(a => new { a.CategoryId, CategoryName = a.Category?.CategoryName ?? "Uncategorized" })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalArticles = g.Count(),
                    ActiveArticles = g.Count(a => a.NewsStatus == true),
                    InactiveArticles = g.Count(a => a.NewsStatus == false)
                })
                .OrderByDescending(s => s.TotalArticles)
                .ToList();

            return new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                CategoryStatistics = categoryStats
            };
        }

        public async Task<object> GetArticleStatisticsByAuthorAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _unitOfWork.NewsArticleRepository.Query()
                .Include(n => n.CreatedBy);

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate);

            var articles = await query.ToListAsync();

            var authorStats = articles
                .GroupBy(a => new { a.CreatedById, AuthorName = a.CreatedBy?.AccountName ?? "Unknown" })
                .Select(g => new
                {
                    AuthorId = g.Key.CreatedById,
                    AuthorName = g.Key.AuthorName,
                    TotalArticles = g.Count(),
                    ActiveArticles = g.Count(a => a.NewsStatus == true),
                    InactiveArticles = g.Count(a => a.NewsStatus == false),
                    LatestArticle = g.OrderByDescending(a => a.CreatedDate).First().CreatedDate
                })
                .OrderByDescending(s => s.TotalArticles)
                .ToList();

            return new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                AuthorStatistics = authorStats
            };
        }

        public async Task<object> GetArticleStatisticsByStatusAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _unitOfWork.NewsArticleRepository.Query();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate);

            var articles = await query.ToListAsync();

            var statusStats = new
            {
                TotalArticles = articles.Count,
                ActiveArticles = articles.Count(a => a.NewsStatus == true),
                InactiveArticles = articles.Count(a => a.NewsStatus == false),
                ActivePercentage = articles.Count > 0 ? 
                    Math.Round((double)articles.Count(a => a.NewsStatus == true) / articles.Count * 100, 2) : 0,
                InactivePercentage = articles.Count > 0 ? 
                    Math.Round((double)articles.Count(a => a.NewsStatus == false) / articles.Count * 100, 2) : 0
            };

            return new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                StatusStatistics = statusStats
            };
        }

        public async Task<object> GetCategoryUsageStatisticsAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.Query()
                .Include(c => c.NewsArticles)
                .ToListAsync();

            var categoryUsage = categories.Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                c.IsActive,
                ArticleCount = c.NewsArticles.Count,
                ActiveArticleCount = c.NewsArticles.Count(a => a.NewsStatus == true),
                InactiveArticleCount = c.NewsArticles.Count(a => a.NewsStatus == false)
            })
            .OrderByDescending(c => c.ArticleCount)
            .ToList();

            return new
            {
                TotalCategories = categories.Count,
                ActiveCategories = categories.Count(c => c.IsActive == true),
                UnusedCategories = categories.Count(c => c.NewsArticles.Count == 0),
                CategoryUsage = categoryUsage
            };
        }

        public async Task<object> GetDashboardStatisticsAsync()
        {
            var articles = await _unitOfWork.NewsArticleRepository.GetAllAsync();
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            var accounts = await _unitOfWork.AccountRepository.GetAllAsync();
            var tags = await _unitOfWork.TagRepository.GetAllAsync();

            var recentArticles = await _unitOfWork.NewsArticleRepository.Query()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .Take(10)
                .ToListAsync();

            return new
            {
                TotalArticles = articles.Count(),
                ActiveArticles = articles.Count(a => a.NewsStatus == true),
                InactiveArticles = articles.Count(a => a.NewsStatus == false),
                TotalCategories = categories.Count(),
                ActiveCategories = categories.Count(c => c.IsActive == true),
                TotalAccounts = accounts.Count(),
                StaffAccounts = accounts.Count(a => a.AccountRole == 1),
                LecturerAccounts = accounts.Count(a => a.AccountRole == 2),
                TotalTags = tags.Count(),
                RecentArticles = recentArticles.Select(a => new
                {
                    a.NewsArticleId,
                    a.NewsTitle,
                    a.CreatedDate,
                    CategoryName = a.Category?.CategoryName,
                    AuthorName = a.CreatedBy?.AccountName,
                    Status = a.NewsStatus == true ? "Active" : "Inactive"
                }).ToList()
            };
        }
    }
}