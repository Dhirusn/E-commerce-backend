using AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Services
{
    public class UserSyncService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserSyncService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> SyncUserAsync(ClaimsPrincipal principal)
        {
            var auth0UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.Identity?.Name;
            var picture = principal.FindFirst("picture")?.Value;
            var nickname= principal.FindFirst("nickname")?.Value;

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId);
            if (existingUser != null) return existingUser;

            var user = new ApplicationUser
            {
                UserName = email ?? nickname,
                Email = email ?? name,
                Auth0UserId = auth0UserId!,
                DisplayName = name?? "",
                ProfileImageUrl = picture??""
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
                throw new Exception("User creation failed: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));

            // ✅ Ensure "User" role exists
            var roleManager = _roleManager; // inject it via constructor
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return user;
        }

    }
}
