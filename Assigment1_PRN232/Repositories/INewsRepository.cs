using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticle?> GetByIdAsync(string id);
        Task AddAsync(NewsArticle news);
        Task UpdateAsync(NewsArticle news);
        Task DeleteAsync(string id);
    }
}