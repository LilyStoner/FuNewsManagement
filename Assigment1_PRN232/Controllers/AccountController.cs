using Assigment1_PRN232_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Assigment1_PRN232_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly FunewsManagementContext _context;
    private readonly IConfiguration _config;

    public AccountController(FunewsManagementContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // Check admin from config first
        var adminSection = _config.GetSection("AdminAccount");
        var adminEmail = adminSection.GetValue<string>("AccountEmail");
        var adminPass = adminSection.GetValue<string>("AccountPassword");

        SystemAccount? account = null;
        if (!string.IsNullOrEmpty(adminEmail) && req.Email == adminEmail && req.Password == adminPass)
        {
            account = new SystemAccount
            {
                AccountId = 0,
                AccountEmail = adminEmail,
                AccountName = "Administrator",
                AccountRole = null,
                AccountPassword = null
            };
        }
        else
        {
            account = await _context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountEmail == req.Email && a.AccountPassword == req.Password);
        }

        if (account == null) return Unauthorized();

        var token = GenerateJwt(account);
        return Ok(new { token });
    }

    private string GenerateJwt(SystemAccount account)
    {
        var jwt = _config.GetSection("Jwt");
        var key = jwt.GetValue<string>("Key");
        var issuer = jwt.GetValue<string>("Issuer");
        var audience = jwt.GetValue<string>("Audience");
        var expireMinutes = jwt.GetValue<int>("ExpireMinutes");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.AccountEmail ?? ""),
            new Claim("id", account.AccountId.ToString())
        };

        var roleClaim = account.AccountRole switch
        {
            1 => new Claim("role", "1"),
            2 => new Claim("role", "2"),
            _ => new Claim("role", "Admin")
        };
        claims.Add(roleClaim);

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ----------------- Admin account management -----------------

    // Get accounts with optional search and role filter. Admin only.
    [HttpGet("admin/accounts")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAccounts([FromQuery] string? search, [FromQuery] string? role)
    {
        var query = _context.SystemAccounts.AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            // allow role as "Staff"/"Lecturer" or numeric
            if (role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                query = query.Where(a => a.AccountRole == 1);
            else if (role.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
                query = query.Where(a => a.AccountRole == 2);
            else if (int.TryParse(role, out var r))
                query = query.Where(a => a.AccountRole == r);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(a => a.AccountName != null && a.AccountName.Contains(search)
                                   || a.AccountEmail != null && a.AccountEmail.Contains(search)
                                   || a.AccountRole != null && a.AccountRole.ToString().Contains(search));
        }

        var list = await query.Select(a => new AccountDto
        {
            AccountId = a.AccountId,
            AccountName = a.AccountName,
            AccountEmail = a.AccountEmail,
            AccountRole = a.AccountRole
        }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("admin/accounts/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAccount(short id)
    {
        var a = await _context.SystemAccounts.FindAsync(id);
        if (a == null) return NotFound();
        return Ok(new AccountDto { AccountId = a.AccountId, AccountName = a.AccountName, AccountEmail = a.AccountEmail, AccountRole = a.AccountRole });
    }

    [HttpPost("admin/accounts")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.AccountEmail) || string.IsNullOrWhiteSpace(req.AccountPassword))
            return BadRequest("Email and password are required.");

        var exists = await _context.SystemAccounts.AnyAsync(a => a.AccountEmail == req.AccountEmail);
        if (exists) return BadRequest("Duplicate email is not allowed.");

        // compute next id (since model uses ValueGeneratedNever)
        var maxId = await _context.SystemAccounts.MaxAsync(a => (int?)a.AccountId) ?? 0;
        var newId = (short)(maxId + 1);

        var account = new SystemAccount
        {
            AccountId = newId,
            AccountName = req.AccountName,
            AccountEmail = req.AccountEmail,
            AccountPassword = req.AccountPassword,
            AccountRole = req.AccountRole
        };

        _context.SystemAccounts.Add(account);
        await _context.SaveChangesAsync();

        var dto = new AccountDto { AccountId = account.AccountId, AccountName = account.AccountName, AccountEmail = account.AccountEmail, AccountRole = account.AccountRole };
        return CreatedAtAction(nameof(GetAccount), new { id = account.AccountId }, dto);
    }

    [HttpPut("admin/accounts/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateAccount(short id, [FromBody] UpdateAccountRequest req)
    {
        var account = await _context.SystemAccounts.FindAsync(id);
        if (account == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.AccountEmail) && req.AccountEmail != account.AccountEmail)
        {
            var exists = await _context.SystemAccounts.AnyAsync(a => a.AccountEmail == req.AccountEmail && a.AccountId != id);
            if (exists) return BadRequest("Duplicate email is not allowed.");
            account.AccountEmail = req.AccountEmail;
        }

        if (!string.IsNullOrWhiteSpace(req.AccountName)) account.AccountName = req.AccountName;
        if (req.AccountRole.HasValue) account.AccountRole = req.AccountRole;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("admin/accounts/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAccount(short id)
    {
        var account = await _context.SystemAccounts.FindAsync(id);
        if (account == null) return NotFound();

        var hasNews = await _context.NewsArticles.AnyAsync(n => n.CreatedById == id);
        if (hasNews) return BadRequest("Account cannot be deleted because it has created news articles.");

        _context.SystemAccounts.Remove(account);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("admin/accounts/{id}/password")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangePassword(short id, [FromBody] ChangePasswordRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.CurrentPassword) || string.IsNullOrWhiteSpace(req.NewPassword))
            return BadRequest("Current and new password are required.");

        var account = await _context.SystemAccounts.FindAsync(id);
        if (account == null) return NotFound();

        if (account.AccountPassword != req.CurrentPassword) return BadRequest("Current password is incorrect.");

        account.AccountPassword = req.NewPassword;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record LoginRequest(string Email, string Password);
public class AccountDto
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
}

public class CreateAccountRequest
{
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public string? AccountPassword { get; set; }
    public int? AccountRole { get; set; }
}

public class UpdateAccountRequest
{
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
}

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}
