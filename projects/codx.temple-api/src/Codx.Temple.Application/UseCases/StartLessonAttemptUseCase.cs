using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class StartLessonAttemptUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public StartLessonAttemptUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<LessonAttemptDto> ExecuteAsync(Guid lessonKey, CancellationToken cancellationToken = default)
    {
        var existing = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.StudentId == _currentUser.UserId
                && a.LessonKey == lessonKey
                && a.Status == Domain.Enums.LessonAttemptStatus.InProgress, cancellationToken);

        if (existing is not null)
        {
            return MapToDto(existing);
        }

        var unresolvedFlags = await _db.AnswerFlags
            .AnyAsync(f => f.StudentId == _currentUser.UserId && f.ResolvedAt == null, cancellationToken);

        if (unresolvedFlags)
            throw new GatingBlockedException(
                "Cannot start a new lesson attempt while unresolved AnswerFlags exist. Complete the current lesson first.",
                []);

        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Key == lessonKey && l.CurrentPublishedVersionId != null, cancellationToken);

        if (lesson is null || lesson.CurrentPublishedVersionId is null)
            throw new NotFoundException(nameof(Lesson), lessonKey);

        var attempt = LessonAttempt.Create(_currentUser.UserId, lessonKey, lesson.CurrentPublishedVersionId.Value);
        _db.LessonAttempts.Add(attempt);
        await _db.SaveChangesAsync(cancellationToken);

        return MapToDto(attempt);
    }

    private static LessonAttemptDto MapToDto(LessonAttempt attempt)
    {
        return new LessonAttemptDto(
            attempt.Id,
            attempt.LessonKey,
            attempt.LessonVersionId,
            attempt.Status.ToString(),
            attempt.StartedAt,
            attempt.CompletedAt,
            []);
    }
}
