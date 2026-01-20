using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Assigment1_PRN232.Services
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
    }
}