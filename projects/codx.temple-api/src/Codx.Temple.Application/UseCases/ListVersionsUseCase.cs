using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListVersionsUseCase
{
    private readonly IAppDbContext _db;

    public ListVersionsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<LessonVersionDto>> ExecuteAsync(Guid lessonKey, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == lessonKey, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), lessonKey);

        var versions = await _db.LessonVersions
            .Where(lv => lv.LessonId == lesson.Id)
            .Include(lv => lv.Nodes)
                .ThenInclude(n => n.Questions)
            .Include(lv => lv.Nodes)
                .ThenInclude(n => n.ChildNodes)
                    .ThenInclude(c => c.Questions)
            .Include(lv => lv.Nodes)
                .ThenInclude(n => n.ChildNodes)
                    .ThenInclude(c => c.ChildNodes)
                        .ThenInclude(g => g.Questions)
            .OrderByDescending(lv => lv.CreatedAt)
            .ToListAsync(cancellationToken);

        return versions.Select(MapToDto).ToList();
    }

    private static LessonVersionDto MapToDto(LessonVersion version)
    {
        var nodeDtos = version.Nodes
            .Where(n => n.ParentNodeId == null)
            .OrderBy(n => n.Order)
            .Select(MapNodeToDto)
            .ToList();

        return new LessonVersionDto(
            version.Id,
            version.LessonId,
            version.VersionNumber,
            version.Status.ToString(),
            version.ChangeNotes,
            version.PublishedAt,
            version.CreatedAt,
            nodeDtos);
    }

    private static LessonNodeDto MapNodeToDto(LessonNode node)
    {
        return new LessonNodeDto(
            node.Id,
            node.Key,
            node.LessonVersionId,
            node.ParentNodeId,
            node.Depth,
            node.Order,
            node.Title,
            node.Description,
            node.RequiresPriorSiblingAnswered,
            node.ChildNodes.OrderBy(c => c.Order).Select(MapNodeToDto).ToList(),
            node.Questions.OrderBy(q => q.Order).Select(q => new QuestionDto(
                q.Id, q.Key, q.LessonNodeId, q.Order,
                q.QuestionType.ToString(), q.PromptText, q.Metadata, q.ReferenceContext)).ToList());
    }
}
