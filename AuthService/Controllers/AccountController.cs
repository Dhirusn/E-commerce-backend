using Microsoft.AspNetCore.Mvc;
using AuthService.Common.Interfaces;
using AuthService.Common.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILoginService _login;

    public AccountController(ILoginService login) => _login = login;
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var res = await _login.RegisterAsync(model);
        if (!res.Succeeded) return BadRequest(res.Errors.Select(e => e.Description));
        return Ok();
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (success, access, refresh, err) = await _login.LoginAsync(model.Email, model.Password, ip);
        if (!success) return Unauthorized(err);
        return Ok(new { accessToken = access, refreshToken = refresh });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (success, access, refresh, err) = await _login.RefreshAsync(model.RefreshToken, ip);
        if (!success) return Unauthorized(err);
        return Ok(new { accessToken = access, refreshToken = refresh });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeDto model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ok = await _login.RevokeRefreshTokenAsync(model.RefreshToken, ip);
        if (!ok) return NotFound();
        return Ok();
    }
    public record LoginDto(string Email, string Password);
    public record RefreshDto(string RefreshToken);
    public record RevokeDto(string RefreshToken);
}
