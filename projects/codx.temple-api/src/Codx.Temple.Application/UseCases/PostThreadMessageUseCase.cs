using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class PostThreadMessageUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public PostThreadMessageUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<ThreadMessageDto> ExecuteAsync(Guid threadId, PostMessageRequest request, CancellationToken ct = default)
    {
        var thread = await _db.AnswerThreads
            .Include(t => t.StudentAnswer)
            .FirstOrDefaultAsync(t => t.Id == threadId, ct)
            ?? throw new Codx.Temple.Application.Exceptions.NotFoundException(nameof(AnswerThread), threadId);

        if (thread.Status == AnswerThreadStatus.Locked)
            throw new InvalidOperationException("Thread is locked");

        var message = ThreadMessage.Create(threadId, _currentUser.UserId, request.BodyText, request.SourceCheckQuestionId);
        _db.ThreadMessages.Add(message);

        var recipientId = _currentUser.UserId == thread.StudentAnswer.StudentId
            ? (Guid?)null // Teacher — need to look up from TeacherAssignment
            : thread.StudentAnswer.StudentId;

        if (recipientId.HasValue)
        {
            var notification = Notification.Create(
                recipientId.Value,
                NotificationType.NewThreadMessage,
                "ThreadMessage", message.Id,
                DeliveryChannel.InApp);
            _db.Notifications.Add(notification);
        }

        await _db.SaveChangesAsync(ct);

        return new ThreadMessageDto(message.Id, message.AuthorId, _currentUser.DisplayName ?? "", message.BodyText, message.SourceCheckQuestionId, message.CreatedAt);
    }
}
