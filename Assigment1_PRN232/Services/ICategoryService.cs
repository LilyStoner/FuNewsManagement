using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232_BE.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(short id);
        Task<IEnumerable<Category>> SearchAsync(string? search);
        Task<Category> CreateAsync(string name, string description, short? parentId, bool? isActive);
        Task UpdateAsync(short id, string? name, string? description, short? parentId, bool? isActive);
        Task DeleteAsync(short id);
        Task<bool> AnyNewsUsingCategoryAsync(short id);
    }
}