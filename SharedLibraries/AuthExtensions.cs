using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibraries.Models;

namespace SharedLibraries
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authSettings = new AuthOptions();
            configuration.Bind(AuthOptions.SectionName, authSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{authSettings.Domain}/";
                options.Audience = authSettings.Audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = $"https://{authSettings.Domain}/",
                    ValidAudience = authSettings.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            return services;
        }
    }
}
