using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Users;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetCurrentUserUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetCurrentUserUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<UserProfileDto> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
            return new UserProfileDto(Guid.Empty, _currentUser.Email ?? "", _currentUser.DisplayName ?? "", [], "Unknown");

        var roles = user.RoleAssignments.Select(r => r.Role.ToString()).ToList();
        return new UserProfileDto(user.Id, user.Email, user.DisplayName, roles, user.Status.ToString());
    }
}
