using Models.Notification;

namespace Models.Notifications.DomineService;

public class CreateInitialNootificcation
{
    public static Notification.Notification BuildWelcomeNotification(Guid profileId,string title,string message,string  alias)
    {
        return Models.Notification.Notification.Create(
            id: Guid.NewGuid(),
            profileId: profileId,
            type: NotificationType.Create("system"),
            data: NotificationData.FromObject(new { title = title, message = message, alias = alias }),
            createdAt:DateTime.UtcNow,
            readAt:null
        );
    }    
}