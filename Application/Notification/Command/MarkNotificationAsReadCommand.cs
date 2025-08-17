using MediatR;
using Models.Notifications.Interface;
using Models.Profiles.Interface;

namespace Application.Notification.Command;

public class MarkNotificationAsReadCommand:IRequest<Guid>
{
    public Guid Id { get; set; }
}


public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Guid>
{
    private readonly INotificationRepository _repo;
    public MarkNotificationAsReadCommandHandler(INotificationRepository repo) => _repo = repo;
    
    
    public async Task<Guid> Handle(MarkNotificationAsReadCommand cmd, CancellationToken cancellationToken)
    {
        var notification =await _repo.GetByIdAsync(cmd.Id, cancellationToken);
        if (notification == null) throw new ArgumentException("No existe notificacion con ese id ");
        notification.MarkAsRead();
        await _repo.UpdateAsync(notification,cancellationToken);
        return notification.Id;
    }
}
