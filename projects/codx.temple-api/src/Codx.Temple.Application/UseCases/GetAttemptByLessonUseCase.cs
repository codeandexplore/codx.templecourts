using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetAttemptByLessonUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetAttemptByLessonUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<LessonAttemptDto?> ExecuteAsync(Guid lessonKey, CancellationToken cancellationToken = default)
    {
        var attempt = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.StudentId == _currentUser.UserId
                && a.LessonKey == lessonKey
                && a.Status == LessonAttemptStatus.InProgress, cancellationToken);

        if (attempt is null)
            return null;

        var answeredKeys = await _db.StudentAnswers
            .Where(a => a.LessonAttemptId == attempt.Id)
            .Select(a => a.QuestionKey)
            .ToListAsync(cancellationToken);

        return new LessonAttemptDto(
            attempt.Id,
            attempt.LessonKey,
            attempt.LessonVersionId,
            attempt.Status.ToString(),
            attempt.StartedAt,
            attempt.CompletedAt,
            answeredKeys);
    }
}
