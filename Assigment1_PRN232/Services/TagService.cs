using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Assigment1_PRN232_BE.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;

        public TagService(ITagRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Tag?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Tag>> SearchAsync(string? search) => await _repo.SearchAsync(search);

        public async Task<Tag> CreateAsync(string name, string? note)
        {
            var existing = (await _repo.SearchAsync(name)).Any(t => string.Equals(t.TagName, name, System.StringComparison.OrdinalIgnoreCase));
            if (existing) throw new InvalidOperationException("Duplicate tag name is not allowed.");

            var maxId = (await _repo.GetAllAsync()).Max(t => (int?)t.TagId) ?? 0;
            var newId = maxId + 1;
            var tag = new Tag { TagId = newId, TagName = name, Note = note };
            await _repo.AddAsync(tag);
            return tag;
        }

        public async Task UpdateAsync(int id, string? name, string? note)
        {
            var tag = await _repo.GetByIdAsync(id);
            if (tag == null) throw new KeyNotFoundException("Tag not found");

            if (!string.IsNullOrWhiteSpace(name) && !string.Equals(name, tag.TagName, System.StringComparison.OrdinalIgnoreCase))
            {
                var exists = (await _repo.SearchAsync(name)).Any(t => t.TagId != id && string.Equals(t.TagName, name, System.StringComparison.OrdinalIgnoreCase));
                if (exists) throw new InvalidOperationException("Duplicate tag name is not allowed.");
                tag.TagName = name;
            }

            if (note != null) tag.Note = note;
            await _repo.UpdateAsync(tag);
        }

        public async Task DeleteAsync(int id)
        {
            if (await _repo.IsTagUsedAsync(id)) throw new InvalidOperationException("Tag cannot be deleted because it is used in NewsTag.");
            await _repo.DeleteAsync(id);
        }

        public async Task<bool> IsTagUsedAsync(int id) => await _repo.IsTagUsedAsync(id);
    }
}