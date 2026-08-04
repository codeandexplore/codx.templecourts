using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.StudentNotes;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CreateStudentQuestionNoteUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public CreateStudentQuestionNoteUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<StudentQuestionNoteDto> ExecuteAsync(
        Guid questionKey,
        CreateNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _db.StudentQuestionNotes
            .FirstOrDefaultAsync(n => n.StudentId == _currentUser.UserId && n.QuestionKey == questionKey, cancellationToken);

        if (existing is not null)
            throw new ConflictException("A note already exists for this question");

        var note = StudentQuestionNote.Create(_currentUser.UserId, questionKey, request.NoteText);
        _db.StudentQuestionNotes.Add(note);
        await _db.SaveChangesAsync(cancellationToken);

        return new StudentQuestionNoteDto(note.Id, note.QuestionKey, note.NoteText, note.CreatedAt);
    }
}
