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
            .OrderBy(n => n.ParentNodeId)
            .ThenBy(n => n.Order)
            .ToListAsync(ct);

        var questions = await _db.Questions
            .Where(q => nodes.Select(n => n.Id).Contains(q.LessonNodeId))
            .ToListAsync(ct);

        var rootNodes = nodes.Where(n => n.ParentNodeId == null).OrderBy(n => n.Order).ToList();
        var orderedKeys = new List<Guid>();
        foreach (var root in rootNodes)
            TraverseTree(root.Id, nodes, questions, orderedKeys);

        return orderedKeys;
    }

    private static void TraverseTree(
        Guid nodeId, List<Domain.Entities.LessonNode> allNodes, List<Domain.Entities.Question> allQuestions, List<Guid> result)
    {
        var children = allNodes.Where(n => n.ParentNodeId == nodeId).OrderBy(n => n.Order).ToList();
        if (children.Count > 0)
        {
            foreach (var child in children)
                TraverseTree(child.Id, allNodes, allQuestions, result);
        }
        else
        {
            var nodeQuestions = allQuestions.Where(q => q.LessonNodeId == nodeId).OrderBy(q => q.Order).Select(q => q.Key);
            result.AddRange(nodeQuestions);
        }
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
