namespace Codx.Temple.Application.Abstractions;

public interface ICurrentUserAccessor
{
    Guid UserId { get; }
    string? Email { get; }
    string? DisplayName { get; }
    IReadOnlyCollection<string> Roles { get; }
    bool IsInRole(string role);
}
