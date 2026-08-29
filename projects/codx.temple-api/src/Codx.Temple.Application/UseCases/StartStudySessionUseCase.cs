using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class StartStudySessionUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public StartStudySessionUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<StudySessionDto> ExecuteAsync(
        StartSessionRequest request, CancellationToken cancellationToken = default)
    {
        var attempt = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.Id == request.LessonAttemptId, cancellationToken)
            ?? throw new NotFoundException(nameof(LessonAttempt), request.LessonAttemptId);

        if (attempt.Status != LessonAttemptStatus.InProgress)
            throw new InvalidOperationException("Lesson attempt is not in progress");

        var existingSessions = await _db.StudySessions
            .Where(s => s.LessonAttemptId == request.LessonAttemptId)
            .OrderBy(s => s.SequenceNumber)
            .ToListAsync(cancellationToken);

        var sequenceNumber = existingSessions.Count + 1;
        Guid? startQuestionId = null;
        Guid? currentQuestionId = null;

        if (existingSessions.Count > 0)
        {
            var lastSession = existingSessions.Last();
            startQuestionId = lastSession.EndQuestionId;
            currentQuestionId = startQuestionId;
        }
        else
        {
            var questions = await GetTreeTraversalQuestionKeys(attempt.LessonVersionId, cancellationToken);
            startQuestionId = questions.FirstOrDefault();
            currentQuestionId = startQuestionId;
        }

        var session = StudySession.Create(attempt.Id, sequenceNumber, startQuestionId, currentQuestionId);
        session.Start();
        _db.StudySessions.Add(session);
        await _db.SaveChangesAsync(cancellationToken);

        return MapToDto(session);
    }

    private async Task<List<Guid>> GetTreeTraversalQuestionKeys(Guid versionId, CancellationToken ct)
    {
        var nodes = await _db.LessonNodes
            .Where(n => n.LessonVersionId == versionId)
            .ToListAsync(ct);

        var questions = await _db.Questions
            .Where(q => nodes.Select(n => n.Id).Contains(q.LessonNodeId))
            .ToListAsync(ct);

        return GetSessionQuestionsUseCase.GetTreeTraversalQuestionKeys(nodes, questions);
    }

    private static StudySessionDto MapToDto(StudySession session)
    {
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
