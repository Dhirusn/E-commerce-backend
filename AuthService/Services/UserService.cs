using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibraries.Auth.JWT;

namespace AuthService.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User already exists with this email"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = registrationDto.Email,
                    Email = registrationDto.Email,
                    DisplayName = registrationDto.DisplayName,
                    PhoneNumber = registrationDto.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registrationDto.Password);

                if (result.Succeeded)
                {
                    // Generate JWT token
                    var token = _tokenService.GenerateJwt(user.Id, user.Email,user.DisplayName);

                    return new AuthResponse
                    {
                        Success = true,
                        Message = "User registered successfully",
                        Token = token,
                        User = new UserResponseDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            DisplayName = user.DisplayName,
                            ProfileImageUrl = user.ProfileImageUrl,
                            EmailConfirmed = user.EmailConfirmed,
                            CreatedAt = user.CreatedAt
                        }
                    };
                }

                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (result.Succeeded)
                {
                    // Generate JWT token
                    var token = _tokenService.GenerateJwt(user.Id, user.Email!, user.DisplayName);

                    return new AuthResponse
                    {
                        Success = true,
                        Message = "Login successful",
                        Token = token,
                        User = new UserResponseDto
                        {
                            Id = user.Id,
                            Email = user.Email!,
                            DisplayName = user.DisplayName,
                            ProfileImageUrl = user.ProfileImageUrl,
                            EmailConfirmed = user.EmailConfirmed,
                            CreatedAt = user.CreatedAt
                        }
                    };
                }

                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                return new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    DisplayName = user.DisplayName,
                    ProfileImageUrl = user.ProfileImageUrl,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(string userId, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                if (!string.IsNullOrEmpty(updateDto.DisplayName))
                    user.DisplayName = updateDto.DisplayName;

                if (!string.IsNullOrEmpty(updateDto.ProfileImageUrl))
                    user.ProfileImageUrl = updateDto.ProfileImageUrl;

                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                    user.PhoneNumber = updateDto.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return false;
            }
        }
    }
}
