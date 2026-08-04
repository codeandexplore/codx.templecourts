using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetLessonTreeUseCase
{
    private readonly IAppDbContext _db;

    public GetLessonTreeUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonVersionDto> ExecuteAsync(
        Guid lessonKey,
        bool includeReferenceContext = true,
        CancellationToken cancellationToken = default)
    {
#pragma warning disable CS8602
        var lesson = await _db.Lessons
            .Include(l => l.CurrentPublishedVersion)
                .ThenInclude(lv => lv.Nodes)
                    .ThenInclude(n => n.Questions)
            .Include(l => l.CurrentPublishedVersion)
                .ThenInclude(lv => lv.Nodes)
                    .ThenInclude(n => n.ChildNodes)
                        .ThenInclude(c => c.Questions)
            .Include(l => l.CurrentPublishedVersion)
                .ThenInclude(lv => lv.Nodes)
                    .ThenInclude(n => n.ChildNodes)
                        .ThenInclude(c => c.ChildNodes)
                            .ThenInclude(g => g.Questions)
            .FirstOrDefaultAsync(l => l.Key == lessonKey, cancellationToken);
#pragma warning restore CS8602

        if (lesson is null || lesson.CurrentPublishedVersion is null)
            throw new NotFoundException(nameof(Domain.Entities.Lesson), lessonKey);

        var version = lesson.CurrentPublishedVersion;
        var nodes = version.Nodes
            .Where(n => n.ParentNodeId == null)
            .OrderBy(n => n.Order)
            .Select(n => MapNode(n, includeReferenceContext))
            .ToList();

        return new LessonVersionDto(
            version.Id,
            version.LessonId,
            version.VersionNumber,
            version.Status.ToString(),
            version.ChangeNotes,
            version.PublishedAt,
            version.CreatedAt,
            nodes);
    }

    private static LessonNodeDto MapNode(Domain.Entities.LessonNode node, bool includeReferenceContext)
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
            node.ChildNodes.OrderBy(c => c.Order).Select(c => MapNode(c, includeReferenceContext)).ToList(),
            node.Questions.OrderBy(q => q.Order).Select(q => new QuestionDto(
                q.Id, q.Key, q.LessonNodeId, q.Order,
                q.QuestionType.ToString(), q.PromptText,
                q.Metadata, includeReferenceContext ? q.ReferenceContext : null)).ToList());
    }
}
