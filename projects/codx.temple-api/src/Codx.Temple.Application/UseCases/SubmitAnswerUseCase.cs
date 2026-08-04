using System.Text.Json;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class SubmitAnswerUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public SubmitAnswerUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<StudentAnswerDto> ExecuteAsync(
        Guid lessonAttemptId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.Id == lessonAttemptId, cancellationToken);

        if (attempt is null)
            throw new NotFoundException(nameof(LessonAttempt), lessonAttemptId);

        if (attempt.StudentId != _currentUser.UserId)
            throw new ForbiddenException("Attempt does not belong to current user");

        if (attempt.Status == LessonAttemptStatus.Completed)
            throw new InvalidOperationException("Attempt is already completed");

        var question = await _db.Questions
            .Include(q => q.LessonNode)
            .FirstOrDefaultAsync(
                q => q.Key == request.QuestionKey && q.LessonNode.LessonVersionId == attempt.LessonVersionId,
                cancellationToken);

        if (question is null)
            throw new NotFoundException(nameof(Question), request.QuestionKey);

        await EnforceSiblingGatingAsync(question, attempt, cancellationToken);

        var answerValueJson = JsonSerializer.SerializeToDocument(request.AnswerValue);

        var existingAnswer = await _db.StudentAnswers
            .FirstOrDefaultAsync(
                a => a.LessonAttemptId == lessonAttemptId && a.QuestionKey == request.QuestionKey,
                cancellationToken);

        StudentAnswer answer;
        if (existingAnswer is not null)
        {
            existingAnswer.UpdateAnswer(answerValueJson);
            answer = existingAnswer;
        }
        else
        {
            answer = StudentAnswer.Create(
                _currentUser.UserId,
                lessonAttemptId,
                request.QuestionKey,
                answerValueJson,
                question.PromptText,
                question.QuestionType.ToString());
            _db.StudentAnswers.Add(answer);
        }

        await _db.SaveChangesAsync(cancellationToken);

        var unresolvedFlag = await _db.AnswerFlags
            .FirstOrDefaultAsync(f => f.QuestionKey == request.QuestionKey
                && f.StudentId == _currentUser.UserId
                && f.ResolvedAt == null, cancellationToken);

        if (unresolvedFlag is not null)
        {
            unresolvedFlag.Resolve();
            await _db.SaveChangesAsync(cancellationToken);
        }

        return new StudentAnswerDto(
            answer.Id,
            answer.LessonAttemptId,
            answer.QuestionKey,
            request.AnswerValue,
            answer.PromptSnapshot,
            answer.QuestionTypeSnapshot,
            answer.SubmittedAt);
    }

    private async Task EnforceSiblingGatingAsync(
        Question question,
        LessonAttempt attempt,
        CancellationToken cancellationToken)
    {
        var node = await _db.LessonNodes
            .FirstOrDefaultAsync(n => n.Id == question.LessonNodeId, cancellationToken);

        if (node is null || !node.RequiresPriorSiblingAnswered)
            return;

        var priorSibling = await _db.LessonNodes
            .FirstOrDefaultAsync(n =>
                n.LessonVersionId == attempt.LessonVersionId &&
                n.ParentNodeId == node.ParentNodeId &&
                n.Order == node.Order - 1,
                cancellationToken);

        if (priorSibling is null)
            return;

        var allNodes = await _db.LessonNodes
            .Where(n => n.LessonVersionId == attempt.LessonVersionId)
            .ToListAsync(cancellationToken);

        var subtreeNodeIds = new List<Guid>();
        CollectSubtreeNodeIds(priorSibling.Id, allNodes, subtreeNodeIds);

        var requiredKeys = await _db.Questions
            .Where(q => subtreeNodeIds.Contains(q.LessonNodeId))
            .Select(q => q.Key)
            .ToListAsync(cancellationToken);

        if (requiredKeys.Count == 0)
            return;

        var answeredKeys = await _db.StudentAnswers
            .Where(a => a.LessonAttemptId == attempt.Id && requiredKeys.Contains(a.QuestionKey))
            .Select(a => a.QuestionKey)
            .ToListAsync(cancellationToken);

        var unsatisfied = requiredKeys.Except(answeredKeys).ToList();
        if (unsatisfied.Count > 0)
        {
            throw new GatingBlockedException(
                "Sibling gating: the immediately preceding sibling node has unanswered questions.",
                unsatisfied);
        }
    }

    private static void CollectSubtreeNodeIds(Guid parentId, List<Domain.Entities.LessonNode> allNodes, List<Guid> result)
    {
        result.Add(parentId);
        foreach (var child in allNodes.Where(n => n.ParentNodeId == parentId))
            CollectSubtreeNodeIds(child.Id, allNodes, result);
    }
}
