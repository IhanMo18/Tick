using Application.Events.Notification;
using Application.Events.Profile;
using Application.Events.User;
using Models.Events;

namespace Application.Events;

public interface IEventTypeRegistry
{
    bool TryResolve(string discriminator, int version, out Type? type);
    (string discriminator, int version) GetInfo(Type t);
}

public sealed class EventTypeRegistry : IEventTypeRegistry
{
    private readonly Dictionary<(string,int), Type> _byDisc = new();
    private readonly Dictionary<Type, (string,int)> _byType = new();

    public EventTypeRegistry()
    {
        Register<ProfileCreatedEvent>("profile.created", 1);
        Register<UserCreatedEventGuid>("user.created",1);
        Register<NotificationCreatedEvent>("notification.created",1);
    }

    private void Register<T>(string disc, int ver) where T : IAppEvent
    {
        _byDisc[(disc, ver)] = typeof(T);
        _byType[typeof(T)] = (disc, ver);
    }

    public bool TryResolve(string disc, int ver, out Type? type)
        => _byDisc.TryGetValue((disc, ver), out type);

    public (string discriminator, int version) GetInfo(Type t)
        => _byType[t];
}