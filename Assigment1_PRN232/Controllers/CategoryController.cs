using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Assigment1_PRN232_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repo;

    public CategoryController(ICategoryRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search)
    {
        var list = await _repo.SearchAsync(search);
        var dto = list.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            CategoryDesciption = c.CategoryDesciption,
            ParentCategoryId = c.ParentCategoryId,
            IsActive = c.IsActive,
            ArticleCount = c.NewsArticles?.Count ?? 0
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(short id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return NotFound();
        return Ok(new CategoryDto { CategoryId = c.CategoryId, CategoryName = c.CategoryName, CategoryDesciption = c.CategoryDesciption, ParentCategoryId = c.ParentCategoryId, IsActive = c.IsActive, ArticleCount = c.NewsArticles?.Count ?? 0 });
    }

    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.CategoryName) || string.IsNullOrWhiteSpace(req.CategoryDesciption))
            return BadRequest("CategoryName and CategoryDesciption are required.");

        // compute id
        var maxId = (await _repo.GetAllAsync()).Max(c => (int?)c.CategoryId) ?? 0;
        var newId = (short)(maxId + 1);

        var cat = new Category { CategoryId = newId, CategoryName = req.CategoryName, CategoryDesciption = req.CategoryDesciption, ParentCategoryId = req.ParentCategoryId, IsActive = req.IsActive };
        await _repo.AddAsync(cat);
        return CreatedAtAction(nameof(Get), new { id = cat.CategoryId }, cat);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Update(short id, [FromBody] UpdateCategoryRequest req)
    {
        var cat = await _repo.GetByIdAsync(id);
        if (cat == null) return NotFound();

        // if category used by articles, cannot change ParentCategoryId
        if (req.ParentCategoryId.HasValue && (await _repo.AnyNewsUsingCategoryAsync(id)))
        {
            return BadRequest("Cannot change ParentCategoryId because this category is used by articles.");
        }

        if (!string.IsNullOrWhiteSpace(req.CategoryName)) cat.CategoryName = req.CategoryName;
        if (!string.IsNullOrWhiteSpace(req.CategoryDesciption)) cat.CategoryDesciption = req.CategoryDesciption;
        if (req.ParentCategoryId.HasValue) cat.ParentCategoryId = req.ParentCategoryId;
        if (req.IsActive.HasValue) cat.IsActive = req.IsActive;

        await _repo.UpdateAsync(cat);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Delete(short id)
    {
        if (await _repo.AnyNewsUsingCategoryAsync(id)) return BadRequest("Category cannot be deleted because it is used by news articles.");
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

public class CategoryDto
{
    public short CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryDesciption { get; set; }
    public short? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
    public int ArticleCount { get; set; }
}

public class CreateCategoryRequest
{
    public string? CategoryName { get; set; }
    public string? CategoryDesciption { get; set; }
    public short? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateCategoryRequest
{
    public string? CategoryName { get; set; }
    public string? CategoryDesciption { get; set; }
    public short? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}
