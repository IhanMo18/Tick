// Application/Notification/Validators/SendNotificationCommandValidator.cs

using System.Text.Json;
using Application.Notification.Command;
using FluentValidation;
using Models.Notification;
using Models.Profiles.Interface;


namespace Application.Notification.Validation;

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        NotificationType.Match.Value,
        NotificationType.Message.Value,
        NotificationType.System.Value
    };

    // Si quieres validar que el perfil exista, inyecta el repo:
    // public SendNotificationCommandValidator(IPerfilRepository perfiles)
    public SendNotificationCommandValidator( IPerfilRepository profiles)
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty().WithMessage("ProfileId es requerido.")
            .MustAsync(profiles.ExistsAsync)
            .WithMessage("El perfil no existe.");

        RuleFor(x => x.NotificationData)
            .NotEmpty().WithMessage("NotificationData es requerido.")
            .MaximumLength(4000).WithMessage("NotificationData demasiado largo.")
            .Must(BeValidJson).WithMessage("NotificationData debe ser JSON válido.");

        RuleFor(x => x.NotificationType.Value)
            .NotEmpty().WithMessage("NotificationType es requerido.")
            .Must(v => AllowedTypes.Contains(v))
            .WithMessage("NotificationType no permitido (usa: match, message, system).");
    }

    private static bool BeValidJson(string s)
    {
        try { JsonDocument.Parse(s); return true; }
        catch { return false; }
    }
}