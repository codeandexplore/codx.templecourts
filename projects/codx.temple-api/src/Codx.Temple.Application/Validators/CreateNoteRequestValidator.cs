using Codx.Temple.Application.DTOs.StudentNotes;
using FluentValidation;

namespace Codx.Temple.Application.Validators;

public sealed class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(x => x.NoteText).NotEmpty();
    }
}
