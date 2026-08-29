using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class UpdateUserStatusUseCase
{
    private readonly IAppDbContext _db;

    public UpdateUserStatusUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid userId, UpdateUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
            throw new ConflictException($"Invalid status: {request.Status}.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), userId);

        if (status == UserStatus.Inactive)
            user.Deactivate();
        else
            user.Activate();

        await _db.SaveChangesAsync(cancellationToken);
    }
}
