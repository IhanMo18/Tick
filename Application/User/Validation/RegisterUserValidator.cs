using Application.User.Command;
using FluentValidation;

namespace Application.User.Validation;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(254);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Debe tener una mayúscula.")
            .Matches("[a-z]").WithMessage("Debe tener una minúscula.")
            .Matches("[0-9]").WithMessage("Debe tener un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Debe tener un carácter especial.");

        RuleFor(x => x.Role).IsInEnum();
    }
}