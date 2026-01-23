using Assigment1_PRN232_BE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Assigment1_PRN232_BE.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string email, string password);
        Task<SystemAccount?> ValidateTokenAsync(string token);
        string GenerateJwtToken(SystemAccount account);
        SystemAccount? GetAdminAccount();
        Task<ClaimsPrincipal?> ValidateJwtTokenAsync(string token);
    }
}