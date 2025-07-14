using Microsoft.AspNetCore.Identity;

namespace AuthService.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Auth0UserId { get; set; } // e.g., "auth0|abc123"
        public string ProfileImageUrl { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
