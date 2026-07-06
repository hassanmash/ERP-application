using Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Infrastructure.Persistence;

namespace Infrastructure.Middlewares
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var isPlatformAdmin = user.FindFirst("is_platform_admin")?.Value == "true";
                var isOrgAdmin = user.FindFirst("is_org_admin")?.Value == "true";
                var orgIdClaim = user.FindFirst("org_id")?.Value;
                var userIdClaim = user.FindFirst("user_id")?.Value;

                Guid? organizationId = null;
                Guid? userId = null;
                if (!isPlatformAdmin)
                {
                    // org_id claim absence for a non-platform-admin is a malformed
                    // token, not a valid "no tenant" state — fail closed.
                    if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var parsedOrgId))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid token: missing or malformed org_id claim.");
                        return;
                    }
                    organizationId = parsedOrgId;
                }

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var parsedUserId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token: missing or malformed user_id claim.");
                    return;
                }
                userId = parsedUserId;

                ((CurrentTenantService)tenantService).SetTenant(organizationId, isPlatformAdmin, userId, isOrgAdmin);
            }

            await _next(context);
        }
    }
}
