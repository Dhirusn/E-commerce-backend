using AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibraries.Auth.JWT;
using SharedLibraries.Models;
using System.Security.Claims;

namespace AuthService.Controllers
{
    public class TokenController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TokenController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] TokenRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) return Unauthorized("User not found");

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordValid.Succeeded) return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            var token = _tokenService.GenerateJwt(user.Id, user.Email, user.UserName, roleClaims);

            return Ok(new { token });
        }
    }
}
