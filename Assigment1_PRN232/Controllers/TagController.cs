using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Assigment1_PRN232_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagRepository _repo;

    public TagController(ITagRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search)
    {
        var list = await _repo.SearchAsync(search);
        var dto = list.Select(t => new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note, ArticleCount = t.NewsArticles?.Count ?? 0 });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return NotFound();
        return Ok(new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note, ArticleCount = t.NewsArticles?.Count ?? 0 });
    }

    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.TagName)) return BadRequest("TagName is required.");

        // prevent duplicate names
        var exists = (await _repo.SearchAsync(req.TagName)).Any(t => string.Equals(t.TagName, req.TagName, StringComparison.OrdinalIgnoreCase));
        if (exists) return BadRequest("Duplicate tag name is not allowed.");

        var maxId = (await _repo.GetAllAsync()).Max(t => (int?)t.TagId) ?? 0;
        var newId = maxId + 1;
        var tag = new Tag { TagId = newId, TagName = req.TagName, Note = req.Note };
        await _repo.AddAsync(tag);
        return CreatedAtAction(nameof(Get), new { id = tag.TagId }, tag);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagRequest req)
    {
        var tag = await _repo.GetByIdAsync(id);
        if (tag == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.TagName) && !string.Equals(req.TagName, tag.TagName, StringComparison.OrdinalIgnoreCase))
        {
            var exists = (await _repo.SearchAsync(req.TagName)).Any(t => t.TagId != id && string.Equals(t.TagName, req.TagName, StringComparison.OrdinalIgnoreCase));
            if (exists) return BadRequest("Duplicate tag name is not allowed.");
            tag.TagName = req.TagName;
        }

        if (req.Note != null) tag.Note = req.Note;
        await _repo.UpdateAsync(tag);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _repo.IsTagUsedAsync(id)) return BadRequest("Tag cannot be deleted because it is used in NewsTag.");
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

public class TagDto
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public string? Note { get; set; }
    public int ArticleCount { get; set; }
}

public class CreateTagRequest
{
    public string? TagName { get; set; }
    public string? Note { get; set; }
}

public class UpdateTagRequest
{
    public string? TagName { get; set; }
    public string? Note { get; set; }
}
