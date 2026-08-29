using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetSessionQuestionsUseCase
{
    private readonly IAppDbContext _db;

    public GetSessionQuestionsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<SessionQuestionsDto> ExecuteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _db.StudySessions
            .Include(s => s.LessonAttempt)
                .ThenInclude(a => a.Student)
            .Include(s => s.LessonAttempt)
                .ThenInclude(a => a.LessonVersion)
                    .ThenInclude(v => v.Lesson)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken)
            ?? throw new NotFoundException(nameof(StudySession), sessionId);

        var attempt = session.LessonAttempt;
        var versionId = attempt.LessonVersionId;

        var nodes = await _db.LessonNodes
            .Where(n => n.LessonVersionId == versionId)
            .ToListAsync(cancellationToken);

        var questions = await _db.Questions
            .Where(q => nodes.Select(n => n.Id).Contains(q.LessonNodeId))
            .ToListAsync(cancellationToken);

        var questionKeys = questions.Select(q => q.Key).ToList();
        var answers = await _db.StudentAnswers
            .Where(a => a.LessonAttemptId == attempt.Id && questionKeys.Contains(a.QuestionKey))
            .ToListAsync(cancellationToken);

        var flags = await _db.AnswerFlags
            .Where(f => f.LessonAttemptId == attempt.Id && questionKeys.Contains(f.QuestionKey) && !f.ResolvedAt.HasValue)
            .Include(f => f.RaisedInSession)
            .ToListAsync(cancellationToken);

        var orderedQuestionKeys = GetTreeTraversalQuestionKeys(nodes, questions);
        var questionDtos = orderedQuestionKeys.Select((key, idx) =>
        {
            var question = questions.First(q => q.Key == key);
            var node = nodes.FirstOrDefault(n => n.Id == question.LessonNodeId);
            var parentTitle = node is not null ? GetNodeBreadcrumb(node, nodes) : "";

            var answer = answers.FirstOrDefault(a => a.QuestionKey == key);
            var flag = flags.FirstOrDefault(f => f.QuestionKey == key);

            return new QuestionWithAnswerDto(
                question.Key,
                idx,
                node?.Depth ?? 1,
                parentTitle,
                question.QuestionType.ToString(),
                question.PromptText,
                answer is not null ? new StudentAnswerInfoDto(
                    answer.AnswerValue.RootElement.GetRawText(),
                    answer.SubmittedAt) : null,
                answer?.Reviewed ?? false,
                flag is not null ? new AnswerFlagInfoDto(
                    flag.FlagType.ToString(),
                    flag.RaisedInSession?.StartedAt ?? DateTimeOffset.UtcNow) : null);
        }).ToList();

        return new SessionQuestionsDto(
            session.Id,
            attempt.Id,
            session.Status.ToString(),
            session.CurrentQuestionId,
            attempt.Student.DisplayName,
            attempt.LessonVersion.Lesson.Number,
            attempt.LessonVersion.Lesson.Title,
            questionDtos);
    }

    public static List<Guid> GetTreeTraversalQuestionKeys(List<LessonNode> nodes, List<Question> questions)
    {
        var rootNodes = nodes.Where(n => n.ParentNodeId == null).OrderBy(n => n.Order).ToList();
        var orderedKeys = new List<Guid>();
        foreach (var root in rootNodes)
            TraverseTree(root.Id, nodes, questions, orderedKeys);
        return orderedKeys;
    }

    private static void TraverseTree(
        Guid nodeId, List<LessonNode> allNodes, List<Question> allQuestions, List<Guid> result)
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

    private static string GetNodeBreadcrumb(LessonNode node, List<LessonNode> allNodes)
    {
        var parts = new List<string>();
        var current = node;
        while (current is not null)
        {
            parts.Insert(0, current.Title);
            current = current.ParentNodeId.HasValue
                ? allNodes.FirstOrDefault(n => n.Id == current.ParentNodeId.Value)
                : null;
        }
        return string.Join(" > ", parts);
    }
}
