using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Assigment1_PRN232_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _service;

    public TagController(ITagService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search)
    {
        var list = await _service.SearchAsync(search);
        var dto = list.Select(t => new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note, ArticleCount = t.NewsArticles?.Count ?? 0 });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var t = await _service.GetByIdAsync(id);
        if (t == null) return NotFound();
        return Ok(new TagDto { TagId = t.TagId, TagName = t.TagName, Note = t.Note, ArticleCount = t.NewsArticles?.Count ?? 0 });
    }

    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(req.TagName)) return BadRequest("TagName is required.");

        try
        {
            var tag = await _service.CreateAsync(req.TagName!, req.Note);
            return CreatedAtAction(nameof(Get), new { id = tag.TagId }, new TagDto { TagId = tag.TagId, TagName = tag.TagName, Note = tag.Note, ArticleCount = tag.NewsArticles?.Count ?? 0 });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, req.TagName, req.Note);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
