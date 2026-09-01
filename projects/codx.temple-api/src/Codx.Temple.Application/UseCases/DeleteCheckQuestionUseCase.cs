using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class DeleteCheckQuestionUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public DeleteCheckQuestionUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var q = await _db.TeacherCheckQuestions
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException(nameof(TeacherCheckQuestion), id);

        if (q.TeacherId != _currentUser.UserId)
            throw new ForbiddenException("Not authorized for this check question");

        _db.TeacherCheckQuestions.Remove(q);
        await _db.SaveChangesAsync(ct);
    }
}