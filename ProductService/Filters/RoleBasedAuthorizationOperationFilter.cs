using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProductService.Filters
{

    public class RoleBasedAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for [Authorize] attributes and hide accordingly
            var authorizeAttributes = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .ToList();

            if (authorizeAttributes.Any())
            {
                // If there's an [Authorize] attribute, check the policy/roles
                var roles = authorizeAttributes
                    .Select(attr => attr.Roles)
                    .Where(roles => !string.IsNullOrEmpty(roles))
                    .FirstOrDefault();

                if (roles != null)
                {
                    operation.Summary += $" [Roles: {roles}]";
                }
            }
        }
    }

}
