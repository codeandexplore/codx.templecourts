using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class UpdateLessonNodeUseCase
{
    private readonly IAppDbContext _db;

    public UpdateLessonNodeUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonNodeDto> ExecuteAsync(Guid versionId, Guid nodeKey, UpdateLessonNodeRequest request, CancellationToken cancellationToken = default)
    {
        var node = await _db.LessonNodes
            .FirstOrDefaultAsync(n => n.Key == nodeKey && n.LessonVersionId == versionId, cancellationToken);

        if (node is null)
            throw new NotFoundException(nameof(Domain.Entities.LessonNode), nodeKey);

        node.Update(request.Title, request.Description, request.RequiresPriorSiblingAnswered);
        await _db.SaveChangesAsync(cancellationToken);

        return new LessonNodeDto(
            node.Id, node.Key, node.LessonVersionId, node.ParentNodeId,
            node.Depth, node.Order, node.Title, node.Description,
            node.RequiresPriorSiblingAnswered, [], []);
    }
}
