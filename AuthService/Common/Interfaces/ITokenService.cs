using AuthService.Data.Models;

namespace AuthService.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
    (string Token, DateTime ExpiresOn, DateTime CreatedOn, string CreatedByIp) CreateRefreshToken(string ipAddress, string userId);
}
