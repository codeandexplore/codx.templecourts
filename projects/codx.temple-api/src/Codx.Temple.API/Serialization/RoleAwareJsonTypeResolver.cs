using System.Text.Json.Serialization.Metadata;
using Codx.Temple.Domain.Entities;

namespace Codx.Temple.API.Serialization;

public sealed class RoleAwareJsonTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleAwareJsonTypeResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        Modifiers.Add(StripReferenceContextForStudents);
    }

    private void StripReferenceContextForStudents(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type != typeof(Question)) return;

        var prop = typeInfo.Properties.FirstOrDefault(
            p => string.Equals(p.Name, "referenceContext", StringComparison.OrdinalIgnoreCase));

        if (prop is null) return;

        prop.ShouldSerialize = (_, _) =>
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return true;
            return !user.IsInRole("Student");
        };
    }
}
