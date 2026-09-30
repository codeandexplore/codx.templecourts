using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class EndStudySessionUseCase
{
    private readonly IAppDbContext _db;

    public EndStudySessionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<StudySessionDto> ExecuteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _db.StudySessions
            .Include(s => s.LessonAttempt)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.StudySession), sessionId);

        if (session.Status != StudySessionStatus.InProgress)
            throw new InvalidOperationException("Session is not in progress");

        session.Complete();
        await _db.SaveChangesAsync(cancellationToken);

        var notification = Notification.Create(
            session.LessonAttempt.StudentId,
            NotificationType.SessionEnded,
            "StudySession", session.Id,
            DeliveryChannel.InApp);
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);

        return new StudySessionDto(
            session.Id,
            session.LessonAttemptId,
            session.SequenceNumber,
            session.StartQuestionId,
            session.EndQuestionId,
            session.CurrentQuestionId,
            session.Status.ToString(),
            session.StartedAt,
            session.EndedAt);
    }
}
