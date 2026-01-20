using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Assigment1_PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<NewsArticle>> Get()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsArticle>> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsArticle news)
        {
            // For skeleton, using a fixed current user id 1. Replace with actual auth user id.
            await _service.AddAsync(news, 1);
            return CreatedAtAction(nameof(Get), new { id = news.NewsArticleId }, news);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticle news)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            news.NewsArticleId = id;
            await _service.UpdateAsync(news, 1);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}