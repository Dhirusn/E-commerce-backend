using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Data.Models;
using AuthService.Common.Interfaces;
using AuthService.Common.Dtos;

namespace AuthService.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenService;

    public LoginService(UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        ApplicationDbContext db,
                        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FullName = dto.FullName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
         //   DateOfBirth = dto.DateOfBirth,
        //    Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            ZipCode = dto.ZipCode,
        //    ProfilePictureUrl = dto.ProfilePictureUrl,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return result;
    }


    public async Task<(bool Success, string? AccessToken, string? RefreshToken, string? Error)> LoginAsync(string email, string password, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return (false, null, null, "Invalid credentials");

        var res = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!res.Succeeded) return (false, null, null, "Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var access = _tokenService.GenerateAccessToken(user, roles);
        var refresh = _tokenService.CreateRefreshToken(ipAddress, user.Id);

        var rt = new RefreshToken
        {
            Token = refresh.Token,
            UserId = user.Id,
            CreatedOn = refresh.CreatedOn,
            CreatedByIp = refresh.CreatedByIp,
            ExpiresOn = refresh.ExpiresOn
        };
        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync();

        return (true, access, refresh.Token, null);
    }

    public async Task<(bool Success, string? AccessToken, string? RefreshToken, string? Error)> RefreshAsync(string refreshToken, string ipAddress)
    {
        var existing = await _db.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == refreshToken);
        if (existing == null || !existing.IsActive) return (false, null, null, "Invalid token");

        existing.RevokedOn = DateTime.UtcNow;
        existing.RevokedByIp = ipAddress;

        var user = existing.User!;
        var roles = await _userManager.GetRolesAsync(user);
        var newAccess = _tokenService.GenerateAccessToken(user, roles);
        var newRefresh = _tokenService.CreateRefreshToken(ipAddress, user.Id);

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefresh.Token,
            UserId = user.Id,
            CreatedOn = newRefresh.CreatedOn,
            CreatedByIp = newRefresh.CreatedByIp,
            ExpiresOn = newRefresh.ExpiresOn
        });

        await _db.SaveChangesAsync();
        return (true, newAccess, newRefresh.Token, null);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
        if (rt == null || !rt.IsActive) return false;
        rt.RevokedOn = DateTime.UtcNow;
        rt.RevokedByIp = ipAddress;
        await _db.SaveChangesAsync();
        return true;
    }
}
