using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shared.Library.Security;

namespace Shared.Library.Identity
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public string UserId => Principal?.GetUserId() ?? string.Empty;
        public string Email => Principal?.GetEmail() ?? string.Empty;
        public string UserName => Principal?.GetUserName() ?? string.Empty;
        public IReadOnlyCollection<string> Roles => Principal?.GetRoles().ToArray() ?? new string[0];
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
    }
}
