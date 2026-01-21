using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Assigment1_PRN232_BE.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticle?> GetByIdAsync(string id);
        Task AddAsync(NewsArticle news);
        Task UpdateAsync(NewsArticle news);
        Task DeleteAsync(string id);
        IQueryable<NewsArticle> GetQueryable();
    }
}