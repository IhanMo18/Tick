using Application.Notification.Command;
using FluentValidation;

namespace Application.Notification.Validation;

public sealed class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id de notificación es requerido.");
    }
}