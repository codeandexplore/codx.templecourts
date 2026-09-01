using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListPublishedQuestionsUseCase
{
    private readonly IAppDbContext _db;

    public ListPublishedQuestionsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<PublishedQuestionDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var publishedVersionIds = await _db.LessonVersions
            .Where(v => v.Status == LessonVersionStatus.Published)
            .Select(v => v.Id)
            .ToListAsync(ct);

        return await _db.Questions
            .Include(q => q.LessonNode)
                .ThenInclude(n => n.LessonVersion)
                    .ThenInclude(v => v.Lesson)
            .Where(q => publishedVersionIds.Contains(q.LessonNode.LessonVersionId))
            .OrderBy(q => q.LessonNode.LessonVersion.Lesson.Number)
            .ThenBy(q => q.PromptText)
            .Select(q => new PublishedQuestionDto(
                q.Key,
                q.PromptText,
                q.LessonNode.LessonVersion.Lesson.Number,
                q.LessonNode.LessonVersion.Lesson.Title))
            .ToListAsync(ct);
    }
}