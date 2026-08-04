using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentNotes;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetStudentQuestionNoteUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetStudentQuestionNoteUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<StudentQuestionNoteDto> ExecuteAsync(Guid questionKey, CancellationToken cancellationToken = default)
    {
        var note = await _db.StudentQuestionNotes
            .FirstOrDefaultAsync(n => n.StudentId == _currentUser.UserId && n.QuestionKey == questionKey, cancellationToken);

        if (note is null)
            throw new NotFoundException(nameof(Domain.Entities.StudentQuestionNote), questionKey);

        return new StudentQuestionNoteDto(note.Id, note.QuestionKey, note.NoteText, note.CreatedAt);
    }
}
