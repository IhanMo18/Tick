using MediatR;

namespace Application.Events;

public interface IAppEvent : INotification
{
    string Discriminator { get; }  // ej: "profile.created"
    int Version { get; }           
    DateTime OccurredOn { get; }
}