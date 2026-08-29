using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class MarkAnswerReviewedUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public MarkAnswerReviewedUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task MarkReviewedAsync(
        MarkReviewedRequest request, CancellationToken cancellationToken = default)
    {
        var answer = await _db.StudentAnswers
            .FirstOrDefaultAsync(a => a.LessonAttemptId == request.LessonAttemptId
                && a.QuestionKey == request.QuestionKey, cancellationToken);

        if (answer is not null && answer.Reviewed)
            return;

        var session = await _db.StudySessions
            .FirstOrDefaultAsync(s => s.LessonAttemptId == request.LessonAttemptId
                && s.Status == StudySessionStatus.InProgress, cancellationToken)
            ?? throw new InvalidOperationException("No active StudySession found. Answers can only be reviewed during a live session.");

        if (answer is null || !answer.HasAnswerValue)
        {
            var attempt = await _db.LessonAttempts
                .FirstOrDefaultAsync(a => a.Id == request.LessonAttemptId, cancellationToken)
                ?? throw new NotFoundException(nameof(LessonAttempt), request.LessonAttemptId);

            var existingFlag = await _db.AnswerFlags
                .FirstOrDefaultAsync(f => f.QuestionKey == request.QuestionKey
                    && f.StudentId == attempt.StudentId
                    && f.ResolvedAt == null, cancellationToken);

            if (existingFlag is null)
            {
                var question = await _db.Questions
                    .FirstOrDefaultAsync(q => q.Key == request.QuestionKey, cancellationToken);

                var flag = AnswerFlag.Create(
                    attempt.StudentId,
                    question?.Id ?? Guid.Empty,
                    request.QuestionKey,
                    request.LessonAttemptId,
                    session.Id);
                _db.AnswerFlags.Add(flag);
                await _db.SaveChangesAsync(cancellationToken);
            }

            throw new GatingBlockedException(
                "Cannot mark unanswered question as reviewed. An AnswerFlag has been raised.",
                [request.QuestionKey]);
        }

        answer.MarkReviewed(_currentUser.UserId, session.Id);
        await _db.SaveChangesAsync(cancellationToken);

        var thread = await _db.AnswerThreads
            .FirstOrDefaultAsync(t => t.StudentAnswerId == answer.Id, cancellationToken);
        if (thread is not null)
        {
            thread.Lock();
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
