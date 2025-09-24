using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Shared.Library.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user) =>
            user?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        public static string GetEmail(this ClaimsPrincipal user) =>
            user?.FindFirstValue(JwtRegisteredClaimNames.Email) ??
            user?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public static string GetUserName(this ClaimsPrincipal user) =>
            user?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal user) =>
            user?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

        public static bool IsInRole(this ClaimsPrincipal user, string role) =>
            user?.IsInRole(role) ?? false;
    }
}
