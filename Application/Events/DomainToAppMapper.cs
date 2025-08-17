using Application.Events.Notification;
using Application.Events.Profile;
using Application.Events.User;
using Models.Events;
using Models.Shared;

namespace Application.Events;

public interface IDomainToAppEventMapper
{
    IEnumerable<IAppEvent> Map(IDomainEvent de);
}

public sealed class DomainToAppEventMapper : IDomainToAppEventMapper
{
    public IEnumerable<IAppEvent> Map(IDomainEvent de) => de switch
    {
        ProfileCreated profileCreated => new IAppEvent[] {
            new ProfileCreatedEvent(DateTime.UtcNow,profileCreated.AliasRaw,profileCreated.ProfileId), 
        },
        
        UserCreated userCreated=>new IAppEvent[]
        {
            new UserCreatedEventGuid(userCreated.UserId,DateTime.UtcNow)    
        },
        
        NotificationCreated notificationCreated=>new IAppEvent[]
        {
            new NotificationCreatedEvent(DateTime.UtcNow)
        } ,
        
        _ => Array.Empty<IAppEvent>()
    };
}