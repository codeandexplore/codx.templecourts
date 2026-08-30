using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CompleteAttemptUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public CompleteAttemptUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<LessonAttemptDto> ExecuteAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        var attempt = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.LessonAttempt), attemptId);

        if (attempt.StudentId != _currentUser.UserId)
            throw new ForbiddenException("Attempt does not belong to current user");

        if (attempt.Status != LessonAttemptStatus.Completed)
        {
            attempt.Complete();
            await _db.SaveChangesAsync(cancellationToken);
        }

        var answeredKeys = await _db.StudentAnswers
            .Where(a => a.LessonAttemptId == attemptId)
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
