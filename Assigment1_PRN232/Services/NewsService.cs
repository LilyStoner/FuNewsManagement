using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Assigment1_PRN232_BE.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repo;

        public NewsService(INewsRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(NewsArticle news, short currentUserId)
        {
            news.CreatedById = currentUserId;
            news.CreatedDate = DateTime.Now;
            await _repo.AddAsync(news);
        }

        public async Task UpdateAsync(NewsArticle news, short currentUserId)
        {
            news.UpdatedById = currentUserId;
            news.ModifiedDate = DateTime.Now;
            await _repo.UpdateAsync(news);
        }

        public async Task DeleteAsync(string id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<NewsArticle>> GetAllPublishedAsync()
        {
            var all = await _repo.GetAllAsync();
            return all.Where(n => n.NewsStatus == true).OrderByDescending(n => n.CreatedDate);
        }

        public async Task<IEnumerable<NewsArticle>> SearchPublishedAsync(string? search, string? role)
        {
            var all = await _repo.GetAllAsync();
            var published = all.Where(n => n.NewsStatus == true);

            if (!string.IsNullOrWhiteSpace(search))
            {
                published = published.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.Headline != null && n.Headline.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.NewsContent != null && n.NewsContent.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (role.Equals("1") || role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    published = published.Where(n => n.CreatedBy != null && n.CreatedBy.AccountRole == 1);
                }
                else if (role.Equals("2") || role.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
                {
                    published = published.Where(n => n.CreatedBy != null && n.CreatedBy.AccountRole == 2);
                }
            }

            return published.OrderByDescending(n => n.CreatedDate);
        }

        public async Task<IEnumerable<NewsArticle>> SearchPublishedAsync(string? search, string? role, DateTime? from, DateTime? to, bool? status)
        {
            var all = await _repo.GetAllAsync();
            var published = all.AsQueryable();

            // status filter (true = published, false = unpublished, null = both)
            if (status.HasValue)
                published = published.Where(n => n.NewsStatus == status.Value);
            else
                published = published.Where(n => n.NewsStatus == true);

            if (!string.IsNullOrWhiteSpace(search))
            {
                published = published.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.Headline != null && n.Headline.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.NewsContent != null && n.NewsContent.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.CreatedBy != null && n.CreatedBy.AccountName != null && n.CreatedBy.AccountName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (n.Category != null && n.Category.CategoryName != null && n.Category.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (role.Equals("1") || role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    published = published.Where(n => n.CreatedBy != null && n.CreatedBy.AccountRole == 1);
                }
                else if (role.Equals("2") || role.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
                {
                    published = published.Where(n => n.CreatedBy != null && n.CreatedBy.AccountRole == 2);
                }
            }

            if (from.HasValue)
            {
                published = published.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value >= from.Value);
            }
            if (to.HasValue)
            {
                published = published.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value <= to.Value);
            }

            return published.OrderByDescending(n => n.CreatedDate).ToList();
        }
    }
}