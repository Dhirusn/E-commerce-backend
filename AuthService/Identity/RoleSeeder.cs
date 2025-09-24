using AuthService.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        var roles = new[] { "Admin", "User" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole(r));
        }

        var adminEmail = "admin@localhost";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = "System Admin",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                CardNumber = "4111111111111111",
                SecurityNumber = "123",
                Expiration = "12/30",
                CardHolderName = "System Admin",
                CardType = 1, // You can map this to an enum later
                Street = "123 Admin Street",
                City = "Admin City",
                State = "Admin State",
                Country = "Admin Country",
                ZipCode = "123456"
            };

            var res = await userManager.CreateAsync(admin, "Admin@12345");
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }

}
