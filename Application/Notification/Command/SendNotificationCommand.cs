using System.Text.Json;
using MediatR;
using Models.Notification;
using Models.Notifications.Interface;


namespace Application.Notification.Command;

public record SendNotificationCommand : IRequest<Guid>
{
    public Guid ProfileId;
    public required string NotificationData;
    public NotificationType NotificationType;
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Guid>
{
    private readonly INotificationRepository _repo;
    
    public SendNotificationCommandHandler(INotificationRepository repo)=>_repo=repo;
    
    
    public async Task<Guid> Handle(SendNotificationCommand cmd, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var jsonData = NotificationData.FromObject(cmd.NotificationData);
        var notification =Models.Notification.Notification.Create(id,cmd.ProfileId,cmd.NotificationType,jsonData,DateTime.Now,null);
        await _repo.AddAsync(notification,cancellationToken);
        return await Task.FromResult(id);
    }
}
