using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class AddLessonNodeUseCase
{
    private readonly IAppDbContext _db;

    public AddLessonNodeUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonNodeDto> ExecuteAsync(Guid versionId, CreateLessonNodeRequest request, CancellationToken cancellationToken = default)
    {
        var version = await _db.LessonVersions
            .FindAsync([versionId], cancellationToken);

        if (version is null)
            throw new NotFoundException(nameof(LessonVersion), versionId);

        LessonNode? parentNode = null;
        int depth = 1;
        int order = 0;

        if (request.ParentNodeKey.HasValue)
        {
            parentNode = await _db.LessonNodes
                .Include(n => n.Questions)
                .FirstOrDefaultAsync(n => n.Key == request.ParentNodeKey.Value && n.LessonVersionId == versionId, cancellationToken);

            if (parentNode is null)
                throw new NotFoundException(nameof(LessonNode), request.ParentNodeKey.Value);

            if (parentNode.Depth >= 3)
                throw new ConflictException("Maximum node depth of 3 reached.");

            if (parentNode.Questions.Count > 0)
                throw new ConflictException("Cannot add a child node to a node that has questions.");

            depth = parentNode.Depth + 1;
            order = await _db.LessonNodes
                .CountAsync(n => n.ParentNodeId == parentNode.Id && n.LessonVersionId == versionId, cancellationToken);
        }
        else
        {
            order = await _db.LessonNodes
                .CountAsync(n => n.ParentNodeId == null && n.LessonVersionId == versionId, cancellationToken);
        }

        var node = LessonNode.Create(
            versionId,
            parentNode?.Id,
            depth,
            request.Order ?? order,
            request.Title,
            request.Description,
            request.RequiresPriorSiblingAnswered);

        _db.LessonNodes.Add(node);
        await _db.SaveChangesAsync(cancellationToken);

        return new LessonNodeDto(
            node.Id, node.Key, node.LessonVersionId, node.ParentNodeId,
            node.Depth, node.Order, node.Title, node.Description,
            node.RequiresPriorSiblingAnswered, [], []);
    }
}
