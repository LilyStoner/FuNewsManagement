using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Assigment1_PRN232_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Category?> GetByIdAsync(short id) => await _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Category>> SearchAsync(string? search) => await _repo.SearchAsync(search);

        public async Task<Category> CreateAsync(string name, string description, short? parentId, bool? isActive)
        {
            var maxId = (await _repo.GetAllAsync()).Max(c => (int?)c.CategoryId) ?? 0;
            var newId = (short)(maxId + 1);
            var cat = new Category { CategoryId = newId, CategoryName = name, CategoryDesciption = description, ParentCategoryId = parentId, IsActive = isActive };
            await _repo.AddAsync(cat);
            return cat;
        }

        public async Task UpdateAsync(short id, string? name, string? description, short? parentId, bool? isActive)
        {
            var cat = await _repo.GetByIdAsync(id);
            if (cat == null) throw new KeyNotFoundException("Category not found");

            if (parentId.HasValue && await _repo.AnyNewsUsingCategoryAsync(id))
            {
                throw new InvalidOperationException("Cannot change ParentCategoryId because this category is used by articles.");
            }

            if (!string.IsNullOrWhiteSpace(name)) cat.CategoryName = name;
            if (!string.IsNullOrWhiteSpace(description)) cat.CategoryDesciption = description;
            if (parentId.HasValue) cat.ParentCategoryId = parentId;
            if (isActive.HasValue) cat.IsActive = isActive;

            await _repo.UpdateAsync(cat);
        }

        public async Task DeleteAsync(short id)
        {
            if (await _repo.AnyNewsUsingCategoryAsync(id)) throw new InvalidOperationException("Category cannot be deleted because it is used by news articles.");
            await _repo.DeleteAsync(id);
        }

        public async Task<bool> AnyNewsUsingCategoryAsync(short id) => await _repo.AnyNewsUsingCategoryAsync(id);
    }
}