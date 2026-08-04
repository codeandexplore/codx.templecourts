using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CreateDraftVersionUseCase
{
    private readonly IAppDbContext _db;

    public CreateDraftVersionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<LessonVersionDto> ExecuteAsync(Guid lessonKey, CreateLessonVersionRequest request, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == lessonKey, cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Lesson), lessonKey);

        var publishedVersion = await _db.LessonVersions
            .Include(lv => lv.Nodes).ThenInclude(n => n.Questions)
            .Include(lv => lv.Nodes).ThenInclude(n => n.ChildNodes).ThenInclude(c => c.Questions)
            .Include(lv => lv.Nodes).ThenInclude(n => n.ChildNodes).ThenInclude(c => c.ChildNodes).ThenInclude(g => g.Questions)
            .FirstOrDefaultAsync(lv => lv.LessonId == lesson.Id && lv.Status == Domain.Enums.LessonVersionStatus.Published, cancellationToken);

        var versionNumber = await _db.LessonVersions
            .CountAsync(lv => lv.LessonId == lesson.Id, cancellationToken) + 1;

        if (publishedVersion is null)
        {
            var emptyDraft = LessonVersion.Create(lesson.Id, versionNumber, request.ChangeNotes);
            _db.LessonVersions.Add(emptyDraft);
            await _db.SaveChangesAsync(cancellationToken);

            return new LessonVersionDto(
                emptyDraft.Id, emptyDraft.LessonId, emptyDraft.VersionNumber,
                emptyDraft.Status.ToString(), emptyDraft.ChangeNotes,
                emptyDraft.PublishedAt, emptyDraft.CreatedAt, []);
        }

        var draftVersion = LessonVersion.Create(lesson.Id, versionNumber, request.ChangeNotes, publishedVersion.Id);

        var nodeMap = new Dictionary<Guid, LessonNode>();
        foreach (var node in publishedVersion.Nodes.Where(n => n.ParentNodeId == null).OrderBy(n => n.Order))
        {
            var clonedNode = CloneNode(node, null, draftVersion.Id, nodeMap);
            draftVersion.Nodes.Add(clonedNode);
            _db.LessonNodes.Add(clonedNode);
        }

        _db.LessonVersions.Add(draftVersion);
        await _db.SaveChangesAsync(cancellationToken);

        return MapToDto(draftVersion);
    }

    private static LessonNode CloneNode(LessonNode source, Guid? parentNodeId, Guid versionId, Dictionary<Guid, LessonNode> map)
    {
        var node = LessonNode.Create(versionId, parentNodeId, source.Depth, source.Order, source.Title, source.Description, source.RequiresPriorSiblingAnswered);
        node.SetKey(source.Key);

        foreach (var question in source.Questions.OrderBy(q => q.Order))
        {
            var q = Question.Create(node.Id, question.Order, question.QuestionType, question.PromptText, CloneJson(question.Metadata), CloneJson(question.ReferenceContext));
            q.SetKey(question.Key);
            node.Questions.Add(q);
        }

        foreach (var child in source.ChildNodes.OrderBy(c => c.Order))
        {
            var clonedChild = CloneNode(child, node.Id, versionId, map);
            node.ChildNodes.Add(clonedChild);
        }

        return node;
    }

    private static System.Text.Json.JsonDocument? CloneJson(System.Text.Json.JsonDocument? source)
    {
        if (source is null) return null;
        return System.Text.Json.JsonDocument.Parse(source.RootElement.GetRawText());
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
