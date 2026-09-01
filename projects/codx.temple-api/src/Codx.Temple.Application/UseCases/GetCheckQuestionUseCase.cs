using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetCheckQuestionUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetCheckQuestionUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<TeacherCheckQuestionDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var q = await _db.TeacherCheckQuestions
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException(nameof(TeacherCheckQuestion), id);

        if (q.TeacherId != _currentUser.UserId)
            throw new ForbiddenException("Not authorized for this check question");

        var isOrphaned = await IsOrphanedAsync(q.QuestionKey, ct);

        return new TeacherCheckQuestionDto(q.Id, q.QuestionKey, q.NoteText, isOrphaned, q.CreatedAt);
    }

    private async Task<bool> IsOrphanedAsync(Guid questionKey, CancellationToken ct)
    {
        var versionIds = await _db.LessonVersions
            .Where(v => v.Status == Domain.Enums.LessonVersionStatus.Published)
            .Select(v => v.Id)
            .ToListAsync(ct);

        var exists = await _db.Questions
            .AnyAsync(q => q.Key == questionKey && versionIds.Contains(q.LessonNode.LessonVersionId), ct);

        return !exists;
    }
}