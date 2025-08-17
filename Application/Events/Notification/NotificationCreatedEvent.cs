using Models.Notification;

namespace Application.Events.Notification;

public sealed record NotificationCreatedEvent(DateTime OccurredOn) : IAppEvent
{
    public string Discriminator => "notification.created";
    public int Version => 1;
}