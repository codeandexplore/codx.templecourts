using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentNotes;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListStudentQuestionNotesUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListStudentQuestionNotesUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<StudentQuestionNoteDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _db.StudentQuestionNotes
            .Where(n => n.StudentId == _currentUser.UserId)
            .Select(n => new StudentQuestionNoteDto(n.Id, n.QuestionKey, n.NoteText, n.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
