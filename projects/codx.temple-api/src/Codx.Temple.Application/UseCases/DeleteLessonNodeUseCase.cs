using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class DeleteLessonNodeUseCase
{
    private readonly IAppDbContext _db;

    public DeleteLessonNodeUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid versionId, Guid nodeKey, CancellationToken cancellationToken = default)
    {
        var node = await _db.LessonNodes
            .FirstOrDefaultAsync(n => n.Key == nodeKey && n.LessonVersionId == versionId, cancellationToken);

        if (node is null)
            throw new NotFoundException(nameof(Domain.Entities.LessonNode), nodeKey);

        var version = await _db.LessonVersions
            .FindAsync([versionId], cancellationToken);

        if (version is not null && version.Status != Domain.Enums.LessonVersionStatus.Draft)
            throw new ConflictException("Can only modify draft versions.");

        _db.LessonNodes.Remove(node);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
