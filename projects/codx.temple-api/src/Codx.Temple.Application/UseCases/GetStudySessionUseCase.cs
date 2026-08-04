using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetStudySessionUseCase
{
    private readonly IAppDbContext _db;

    public GetStudySessionUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<StudySessionDto> ExecuteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _db.StudySessions
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.StudySession), sessionId);

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
