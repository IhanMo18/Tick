using Models.Events;
using Models.Shared;

namespace Models.Notification;

public sealed class Notification:AggregateRoot
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public NotificationType Type { get; private set; }
    public NotificationData Data { get; set; }
    public bool IsRead{ get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; private set; }
    
    
    private Notification() {}

    private Notification(Guid id, Guid profileId, NotificationType type, NotificationData data)
    {
        Id = id;
        ProfileId = profileId;
        Type = type;
        Data = data;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static Notification Create(Guid id,Guid profileId, NotificationType type, NotificationData data,DateTime createdAt,DateTime? readAt) 
    {
        var notification = new Notification(id,profileId, type, data);
        notification.Raise(new NotificationCreated(profileId,type,data));
        return notification;
    }
    
    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        Raise(new NotificationReaded(Id, ProfileId));
    }
}

