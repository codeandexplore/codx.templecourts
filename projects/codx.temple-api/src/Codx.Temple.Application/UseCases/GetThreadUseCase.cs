using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetThreadUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetThreadUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<AnswerThreadDto> ExecuteAsync(Guid threadId, CancellationToken ct = default)
    {
        var thread = await _db.AnswerThreads
            .Include(t => t.StudentAnswer)
            .Include(t => t.Messages)
                .ThenInclude(m => m.Author)
            .FirstOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new NotFoundException(nameof(AnswerThread), threadId);

        await EnsureParticipantAsync(thread, ct);

        var messages = thread.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ThreadMessageDto(
                m.Id, m.AuthorId, m.Author.DisplayName, m.BodyText, m.SourceCheckQuestionId, m.CreatedAt))
            .ToList();

        return new AnswerThreadDto(
            thread.Id,
            thread.StudentAnswerId,
            thread.Status.ToString(),
            thread.LockedAt,
            messages);
    }

    protected async Task EnsureParticipantAsync(AnswerThread thread, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (thread.StudentAnswer.StudentId == userId)
            return;

        var isTeacher = await _db.TeacherAssignments
            .AnyAsync(a => a.StudentId == thread.StudentAnswer.StudentId
                && a.PrimaryTeacherId == userId
                && a.Status == TeacherAssignmentStatus.Active, ct);

        if (!isTeacher)
            throw new ForbiddenException("Not authorized for this thread");
    }
}