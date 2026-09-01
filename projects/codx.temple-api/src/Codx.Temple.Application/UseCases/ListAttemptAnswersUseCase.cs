using System.Text.Json;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListAttemptAnswersUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListAttemptAnswersUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<StudentAnswerDto>> ExecuteAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        var attempt = await _db.LessonAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.LessonAttempt), attemptId);

        if (attempt.StudentId != _currentUser.UserId)
            throw new ForbiddenException("Attempt does not belong to current user");

        var answers = await _db.StudentAnswers
            .Where(a => a.LessonAttemptId == attemptId)
            .OrderBy(a => a.SubmittedAt)
            .ToListAsync(cancellationToken);

        var answerIds = answers.Select(a => a.Id).ToList();
        var threadByAnswerId = await _db.AnswerThreads
            .Where(t => answerIds.Contains(t.StudentAnswerId))
            .Select(t => new { t.StudentAnswerId, t.Id })
            .ToDictionaryAsync(t => t.StudentAnswerId, t => (Guid?)t.Id, cancellationToken);

        return answers.Select(a => new StudentAnswerDto(
            a.Id,
            a.LessonAttemptId,
            a.QuestionKey,
            ExtractValue(a.AnswerValue),
            a.PromptSnapshot,
            a.QuestionTypeSnapshot,
            a.SubmittedAt,
            threadByAnswerId.GetValueOrDefault(a.Id))).ToList();
    }

    private static object ExtractValue(JsonDocument document)
    {
        var root = document.RootElement;
        return root.ValueKind switch
        {
            JsonValueKind.String => root.GetString()!,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => root.GetRawText(),
            JsonValueKind.Object or JsonValueKind.Array => root.GetRawText(),
            _ => root.GetRawText()
        };
    }
}
