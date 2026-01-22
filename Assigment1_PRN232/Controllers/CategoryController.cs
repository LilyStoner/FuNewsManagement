using Assigment1_PRN232_BE.Models;
using Assigment1_PRN232_BE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Assigment1_PRN232_BE.DTOs;

namespace Assigment1_PRN232_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    private short GetCurrentUserId()
    {
        var idClaim = User.FindFirst("id")?.Value;
        return short.TryParse(idClaim, out var id) ? id : (short)1;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search)
    {
        var list = await _service.SearchAsync(search);
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
        var c = await _service.GetByIdAsync(id);
        if (c == null) return NotFound();
        return Ok(new CategoryDto { CategoryId = c.CategoryId, CategoryName = c.CategoryName, CategoryDesciption = c.CategoryDesciption, ParentCategoryId = c.ParentCategoryId, IsActive = c.IsActive, ArticleCount = c.NewsArticles?.Count ?? 0 });
    }

    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(req.CategoryName) || string.IsNullOrWhiteSpace(req.CategoryDesciption))
            return BadRequest(new { message = "CategoryName and CategoryDesciption are required." });

        try
        {
            var cat = await _service.CreateAsync(req.CategoryName!, req.CategoryDesciption!, req.ParentCategoryId, req.IsActive);
            var dto = new CategoryDto { CategoryId = cat.CategoryId, CategoryName = cat.CategoryName, CategoryDesciption = cat.CategoryDesciption, ParentCategoryId = cat.ParentCategoryId, IsActive = cat.IsActive, ArticleCount = cat.NewsArticles?.Count ?? 0 };
            return CreatedAtAction(nameof(Get), new { id = cat.CategoryId }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> Update(short id, [FromBody] UpdateCategoryRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, req.CategoryName, req.CategoryDesciption, req.ParentCategoryId, req.IsActive);
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
    public async Task<IActionResult> Delete(short id)
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
