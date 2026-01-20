using Assigment1_PRN232_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assigment1_PRN232.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly FunewsManagementContext _context;

        public NewsRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles.Include(n => n.Category).Include(n => n.Tags).ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _context.NewsArticles.Include(n => n.Category).Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        public async Task AddAsync(NewsArticle news)
        {
            _context.NewsArticles.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsArticle news)
        {
            _context.NewsArticles.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.NewsArticles.FindAsync(id);
            if (entity != null)
            {
                _context.NewsArticles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}