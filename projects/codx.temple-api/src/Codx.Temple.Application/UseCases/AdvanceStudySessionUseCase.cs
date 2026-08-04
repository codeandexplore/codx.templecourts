using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class AdvanceStudySessionUseCase
{
    private readonly IAppDbContext _db;

    public AdvanceStudySessionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<StudySessionDto> ExecuteAsync(
        Guid sessionId,
        AdvanceSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = await _db.StudySessions
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.StudySession), sessionId);

        if (session.Status != StudySessionStatus.InProgress)
            throw new InvalidOperationException("Session is not in progress");

        session.Advance(request.CurrentQuestionId);
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
