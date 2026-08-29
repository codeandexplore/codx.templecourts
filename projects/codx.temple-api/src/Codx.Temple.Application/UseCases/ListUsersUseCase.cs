using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListUsersUseCase
{
    private readonly IAppDbContext _db;

    public ListUsersUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<UserDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _db.Users
            .Include(u => u.RoleAssignments)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.DisplayName,
            u.Status.ToString(),
            u.RoleAssignments.Select(r => r.Role.ToString()).ToList())).ToList();
    }
}
