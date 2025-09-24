using System.Collections.Generic;

namespace Shared.Library.Identity
{
    public interface IUserContextService
    {
        string UserId { get; }
        string Email { get; }
        string UserName { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
