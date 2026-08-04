using Codx.Temple.Application.DTOs.StudentNotes;
using FluentValidation;

namespace Codx.Temple.Application.Validators;

public sealed class UpdateNoteRequestValidator : AbstractValidator<UpdateNoteRequest>
{
    public UpdateNoteRequestValidator()
    {
        RuleFor(x => x.NoteText).NotEmpty();
    }
}
