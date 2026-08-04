using Microsoft.AspNetCore.Authorization;

namespace Codx.Temple.API.Authorization;

public sealed class RequireRoleRequirement : IAuthorizationRequirement
{
    public string[] Roles { get; }

    public RequireRoleRequirement(params string[] roles)
    {
        Roles = roles;
    }
}

public sealed class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
        Policy = $"RequireRole_{string.Join("_", roles)}";
    }
}
