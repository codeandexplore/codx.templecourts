using Codx.Temple.Application.DTOs.Auth;
using FluentValidation;

namespace Codx.Temple.Application.Validators;

public sealed class GoogleAuthRequestValidator : AbstractValidator<GoogleAuthRequest>
{
    public GoogleAuthRequestValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
}
