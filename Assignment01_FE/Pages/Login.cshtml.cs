using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

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

        // store token in session
        HttpContext.Session.SetString("auth_token", payload.token);

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
