using Assigment1_PRN232_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Assigment1_PRN232_BE.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly FunewsManagementContext _context;

        public NewsRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public IQueryable<NewsArticle> GetQueryable()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Include(n => n.CreatedBy)
                .AsNoTracking();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await GetQueryable().ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _context.NewsArticles.Include(n => n.Category).Include(n => n.Tags).Include(n => n.CreatedBy)
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
            var entity = await _context.NewsArticles.Include(n => n.Tags).FirstOrDefaultAsync(n => n.NewsArticleId == id);
            if (entity != null)
            {
                // remove relations in join table (NewsTag)
                entity.Tags.Clear();
                await _context.SaveChangesAsync();

                _context.NewsArticles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}