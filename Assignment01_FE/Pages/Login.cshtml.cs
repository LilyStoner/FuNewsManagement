using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Assignment01_FE.Pages;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpFactory;

    public LoginModel(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpFactory.CreateClient();
        var res = await client.PostAsJsonAsync("https://localhost:7215/api/account/login", new { Email = Input.Email, Password = Input.Password });
        if (!res.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid credentials";
            return Page();
        }

        var payload = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (payload?.token == null)
        {
            ErrorMessage = "Invalid response from server";
            return Page();
        }

        // Parse JWT token to extract claims
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(payload.token);
        var roleClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
        var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        var nameClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        // Store token and user info in session
        HttpContext.Session.SetString("auth_token", payload.token);
        HttpContext.Session.SetString("auth_role", roleClaim ?? "");
        HttpContext.Session.SetString("auth_user_id", userIdClaim ?? "");
        HttpContext.Session.SetString("auth_user_name", nameClaim ?? "");

        return RedirectToPage("/News");
    }

    public class InputModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    private class LoginResponse { public string? token { get; set; } }
}
