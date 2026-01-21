using Assigment1_PRN232_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232_BE.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<IEnumerable<Tag>> SearchAsync(string? search);
        Task<Tag> CreateAsync(string name, string? note);
        Task UpdateAsync(int id, string? name, string? note);
        Task DeleteAsync(int id);
        Task<bool> IsTagUsedAsync(int id);
    }
}