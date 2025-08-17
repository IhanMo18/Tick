using Application.Profile.Command;
using FluentValidation;
using Models.Profiles.Interface;

namespace Application.Profile.Validation;

public sealed class ChangeStatusCommandValidator : AbstractValidator<ChangeStatusCommand>
{
    public ChangeStatusCommandValidator(IPerfilRepository profiles)
    {
        RuleFor(x => x.id).NotEmpty();
        RuleFor(x => x.status).IsInEnum();
        
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("ProfileId es requerido.")
            .MustAsync(profiles.ExistsAsync)
            .WithMessage("El perfil no existe.");
    }
}