using Codx.Temple.Application.DTOs.StudentAttempts;
using FluentValidation;

namespace Codx.Temple.Application.Validators;

public sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    public SubmitAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionKey).NotEmpty();
        RuleFor(x => x.AnswerValue).NotNull();
    }
}
