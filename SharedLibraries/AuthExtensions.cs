using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibraries.Models;
using System.Security.Claims;

namespace SharedLibraries
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authSettings = new AuthOptions();
            configuration.Bind(AuthOptions.SectionName, authSettings);

            // Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://dev-zn7kooyuqtsoiajl.us.auth0.com/";
                options.Audience = "https://product-api";
            });

            // Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "read:messages",
                    policy => policy.Requirements.Add(
                        new HasScopeRequirement("read:messages", authSettings.Domain)
                    )
                );
            });

            // Register custom scope handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            return services;
        }
    }
}
