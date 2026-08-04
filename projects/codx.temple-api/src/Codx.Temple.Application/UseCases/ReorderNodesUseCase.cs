using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ReorderNodesUseCase
{
    private readonly IAppDbContext _db;

    public ReorderNodesUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid versionId, ReorderNodesRequest request, CancellationToken cancellationToken = default)
    {
        var nodes = await _db.LessonNodes
            .Where(n => n.LessonVersionId == versionId &&
                        (request.ParentNodeKey.HasValue
                            ? n.ParentNode != null && n.ParentNode.Key == request.ParentNodeKey.Value
                            : n.ParentNodeId == null))
            .ToListAsync(cancellationToken);

        for (int i = 0; i < request.OrderedKeys.Count; i++)
        {
            var node = nodes.FirstOrDefault(n => n.Key == request.OrderedKeys[i]);
            if (node is not null)
                node.SetOrder(i);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
