using Microsoft.Extensions.DependencyInjection;
using Shared.Library.Identity;
using Shared.Library.Audit;
using Shared.Library.Tokens;

namespace Shared.Library.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedLibrary(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddSingleton<IRefreshTokenRepository, InMemoryRefreshTokenRepository>();
            return services;
        }
    }
}
