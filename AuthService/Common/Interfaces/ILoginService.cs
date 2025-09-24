using AuthService.Common.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Common.Interfaces;

public interface ILoginService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string? AccessToken, string? RefreshToken, string? Error)> LoginAsync(string email, string password, string ipAddress);
    Task<(bool Success, string? AccessToken, string? RefreshToken, string? Error)> RefreshAsync(string refreshToken, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
}
