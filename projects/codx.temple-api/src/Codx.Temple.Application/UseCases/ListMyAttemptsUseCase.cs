using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListMyAttemptsUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListMyAttemptsUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<StudentAttemptSummaryDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var attempts = await _db.LessonAttempts
            .Where(a => a.StudentId == _currentUser.UserId)
            .Include(a => a.LessonVersion)
                .ThenInclude(v => v.Lesson)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);

        var attemptIds = attempts.Select(a => a.Id).ToList();
        var answerCounts = await _db.StudentAnswers
            .Where(a => attemptIds.Contains(a.LessonAttemptId))
            .GroupBy(a => a.LessonAttemptId)
            .Select(g => new { AttemptId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AttemptId, x => x.Count, cancellationToken);

        return attempts.Select(a => new StudentAttemptSummaryDto(
            a.Id,
            a.LessonKey,
            a.LessonVersion.Lesson.Number,
            a.LessonVersion.Lesson.Title,
            a.Status.ToString(),
            a.StartedAt,
            a.CompletedAt,
            answerCounts.TryGetValue(a.Id, out var count) ? count : 0)).ToList();
    }
}
