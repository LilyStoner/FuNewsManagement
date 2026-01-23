using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Services
{
    public interface IReportService
    {
        Task<object> GetArticleStatisticsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<object> GetArticleStatisticsByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetArticleStatisticsByAuthorAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetArticleStatisticsByStatusAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetCategoryUsageStatisticsAsync();
        Task<object> GetDashboardStatisticsAsync();
    }
}