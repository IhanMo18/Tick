using System.Text.Json;
using Models.Users;

namespace Application.Events.User;

public sealed record UserCreatedEventGuid (Guid UserId, DateTime OccurredOn ): IAppEvent
{
    
    public string Discriminator => "user.created";
    public int Version => 1;
}