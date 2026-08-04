using Microsoft.AspNetCore.Authorization;

namespace Codx.Temple.API.Authorization;

public sealed class RequireRoleHandler : AuthorizationHandler<RequireRoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequireRoleRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        var userRoles = context.User.FindFirst("application_role")?.Value?.Split(',')
            ?? [];

        if (requirement.Roles.Any(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase)))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
