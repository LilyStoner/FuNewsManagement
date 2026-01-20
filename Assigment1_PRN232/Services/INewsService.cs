using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticle?> GetByIdAsync(string id);
        Task AddAsync(NewsArticle news, short currentUserId);
        Task UpdateAsync(NewsArticle news, short currentUserId);
        Task DeleteAsync(string id);
    }
}