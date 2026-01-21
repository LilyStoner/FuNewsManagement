using Assigment1_PRN232_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assigment1_PRN232_BE.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FunewsManagementContext _context;

        public CategoryRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Include(c => c.NewsArticles).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(short id)
        {
            return await _context.Categories.Include(c => c.NewsArticles).FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity != null)
            {
                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AnyNewsUsingCategoryAsync(short id)
        {
            return await _context.NewsArticles.AnyAsync(n => n.CategoryId == id);
        }

        public async Task<IEnumerable<Category>> SearchAsync(string? search)
        {
            var q = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(c => (c.CategoryName != null && c.CategoryName.Contains(search)) || (c.CategoryDesciption != null && c.CategoryDesciption.Contains(search)));
            }
            return await q.Include(c => c.NewsArticles).ToListAsync();
        }
    }
}
