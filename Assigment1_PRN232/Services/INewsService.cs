using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Assigment1_PRN232_BE.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<IEnumerable<NewsArticle>> GetAllPublishedAsync();
        Task<IEnumerable<NewsArticle>> SearchPublishedAsync(string? search, string? role);
        Task<IEnumerable<NewsArticle>> SearchPublishedAsync(string? search, string? role, System.DateTime? from, System.DateTime? to, bool? status);
        Task<NewsArticle?> GetByIdAsync(string id);
        Task AddAsync(NewsArticle news, short currentUserId);
        Task UpdateAsync(NewsArticle news, short currentUserId);
        Task DeleteAsync(string id);

        // IQueryable-based methods for OData/server-side composition
        IQueryable<NewsArticle> GetPublishedQueryable();
        IQueryable<NewsArticle> SearchPublishedQueryable(string? search, string? role, System.DateTime? from, System.DateTime? to, bool? status);
    }
}