using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232_BE.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(short id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(short id);
        Task<bool> AnyNewsUsingCategoryAsync(short id);
        Task<IEnumerable<Category>> SearchAsync(string? search);
    }
}
