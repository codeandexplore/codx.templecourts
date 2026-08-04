using Codx.Temple.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class DeleteStudentQuestionNoteUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public DeleteStudentQuestionNoteUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task ExecuteAsync(Guid questionKey, CancellationToken cancellationToken = default)
    {
        var note = await _db.StudentQuestionNotes
            .FirstOrDefaultAsync(n => n.StudentId == _currentUser.UserId && n.QuestionKey == questionKey, cancellationToken);

        if (note is not null)
        {
            _db.StudentQuestionNotes.Remove(note);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
