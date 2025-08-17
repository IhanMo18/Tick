using Application.Abstractions;
using Application.Events.Profile;
using MediatR;
using Models.Notification;
using Models.Notifications.DomineService;
using Models.Notifications.Interface;
using Models.Profiles;
using Models.Profiles.Interface;

namespace Application.Profile.Integration;

public record ProfileRegisteredIntegrationEventSendNotification(
    Guid ProfileId,string AliasRaw
) : INotification;


public sealed class CreateWelcomeNotificationHandler
    : INotificationHandler<ProfileCreatedEvent>
{
    private readonly INotificationRepository _notifs;
    private readonly IPerfilRepository _perfiles;
    private readonly IUnitOfWork _uow;

    public CreateWelcomeNotificationHandler(
        INotificationRepository notifs,
        IPerfilRepository perfiles,
        IUnitOfWork uow)
    {
        _notifs = notifs;
        _perfiles = perfiles;
        _uow = uow;
    }

    public async Task Handle(ProfileCreatedEvent e, CancellationToken ct)
    {
        
        var payload = new {
            title = $@"¡Hola {e.AliasRaw}!",
            message = "Gracias por registrarte en Tick",
            alias = e.AliasRaw
        };
        
        var profile = await _perfiles.GetByIdAsync(e.ProfileId, ct);
        if (profile is null) return;
        
        var alreadyExists = await _notifs.ExistsAsync(
            profile.Id,
            NotificationType.System,
            payload:payload,
            ct);

        if (alreadyExists) return;
        
      var notif= CreateInitialNootificcation.BuildWelcomeNotification(e.ProfileId,payload.title,payload.message,payload.alias);

        await _notifs.AddAsync(notif, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
