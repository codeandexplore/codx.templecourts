using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListCheckQuestionsUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListCheckQuestionsUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<TeacherCheckQuestionDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var items = await _db.TeacherCheckQuestions
            .Where(q => q.TeacherId == _currentUser.UserId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(ct);

        var publishedKeys = await GetPublishedQuestionKeysAsync(ct);

        return items.Select(q => new TeacherCheckQuestionDto(
            q.Id,
            q.QuestionKey,
            q.NoteText,
            !publishedKeys.Contains(q.QuestionKey),
            q.CreatedAt)).ToList();
    }

    private async Task<HashSet<Guid>> GetPublishedQuestionKeysAsync(CancellationToken ct)
    {
        var versionIds = await _db.LessonVersions
            .Where(v => v.Status == Domain.Enums.LessonVersionStatus.Published)
            .Select(v => v.Id)
            .ToListAsync(ct);

        return await _db.Questions
            .Where(q => versionIds.Contains(q.LessonNode.LessonVersionId))
            .Select(q => q.Key)
            .ToHashSetAsync(ct);
    }
}