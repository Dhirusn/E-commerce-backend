using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.ViewModels;
using System.Linq;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json;
using System.Text;

namespace AuthService.Controllers
{
    public class AccountController : Controller
    {
        public readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Login(string returnUrl = "/")
        {
            var props = new LoginAuthenticationPropertiesBuilder().WithRedirectUri(returnUrl).Build();
            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, props);
        }

        [Authorize]
        public async Task Logout()
        {
            var props = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(Url.Action("Index", "Home"))
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, props);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("/account/me")]
        public IActionResult Me()
        {
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            return Ok(new
            {
                Name = User.Identity?.Name,
                Email = claims.GetValueOrDefault("email"),
                Picture = claims.GetValueOrDefault("picture"),
                Roles = claims.Where(c => c.Key == "role").Select(c => c.Value).ToList()
            });
        }
        [Authorize]
        public IActionResult Profile()
        {
            return View(new UserProfileViewModel
            {
                Name = User.Identity!.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }


        /// <summary>
        /// This is just a helper action to enable you to easily see all claims related to a user. It helps when debugging your
        /// application to see the in claims populated from the Auth0 ID Token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost("/account/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var client = new HttpClient();
            var domain = _configuration["Auth0:Domain"];
            var connection = _configuration["Auth0:Connection"]; // usually "Username-Password-Authentication"

            var requestPayload = new
            {
                client_id = _configuration["Auth0:ClientId"],
                email = model.Email,
                connection = connection
            };

            var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://{domain}/dbconnections/change_password", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Reset password email sent." });
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(new { message = "Failed to send reset email", error });
        }

    }
}
