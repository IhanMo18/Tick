using System.Text.Json;
using Models.Notification;
using Models.Shared;

namespace Models.Events;

public record NotificationCreated(Guid ProfileId, NotificationType Type, NotificationData Data) : IDomainEvent
{
    public DateTime OccurredOn{ get;}= DateTime.UtcNow;

    public string Payload => JsonSerializer.Serialize(new
    {
        Type,
        Data,
        ProfileId,
        OccurredOn
    });
};

public record NotificationReaded(Guid NotificationId,Guid ProfileId) : IDomainEvent
{
    public DateTime OccurredOn{ get;}= DateTime.UtcNow;

    public string Payload => JsonSerializer.Serialize(new
    {
        NotificationId,
        ProfileId,
        OccurredOn
    });
};