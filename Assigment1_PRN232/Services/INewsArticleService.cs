using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Services
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsArticlesAsync();
        Task<NewsArticle?> GetNewsArticleByIdAsync(string id);
        Task<IEnumerable<NewsArticle>> GetActiveNewsArticlesAsync();
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByAuthorAsync(short authorId);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByCategoryAsync(short categoryId);
        Task<NewsArticle> CreateNewsArticleAsync(NewsArticle article, IEnumerable<int>? tagIds = null);
        Task<NewsArticle> UpdateNewsArticleAsync(NewsArticle article, IEnumerable<int>? tagIds = null);
        Task<bool> DeleteNewsArticleAsync(string id);
        Task<IEnumerable<NewsArticle>> SearchNewsArticlesAsync(
            string? title = null, 
            string? authorName = null, 
            string? categoryName = null, 
            bool? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<NewsArticle> DuplicateArticleAsync(string originalId, short newAuthorId);
        Task<IEnumerable<NewsArticle>> GetRelatedNewsAsync(string articleId, int limit = 3);
        string GenerateNewsArticleId();
        IQueryable<NewsArticle> GetNewsArticlesQueryable();
    }
}