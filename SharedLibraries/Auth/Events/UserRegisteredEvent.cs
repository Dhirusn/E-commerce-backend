using System;

namespace Shared.Library.Events
{
    public sealed record UserRegisteredEvent
    (
        string UserId,
        string Email,
        string FullName,
        DateTime RegisteredAt
    );
}
