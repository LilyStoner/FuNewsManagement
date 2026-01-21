using Assigment1_PRN232_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assigment1_PRN232_BE.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FunewsManagementContext _context;

        public TagRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.Include(t => t.NewsArticles).ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.Include(t => t.NewsArticles).FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task AddAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Tags.FindAsync(id);
            if (entity != null)
            {
                _context.Tags.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTagUsedAsync(int id)
        {
            return await _context.NewsArticles.AnyAsync(n => n.Tags.Any(t => t.TagId == id));
        }

        public async Task<IEnumerable<Tag>> SearchAsync(string? search)
        {
            var q = _context.Tags.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(t => t.TagName != null && t.TagName.Contains(search));
            return await q.Include(t => t.NewsArticles).ToListAsync();
        }
    }
}
