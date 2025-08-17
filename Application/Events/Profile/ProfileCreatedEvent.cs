using System.Text.Json;

namespace Application.Events.Profile;

public sealed record ProfileCreatedEvent(
    DateTime OccurredOn,string AliasRaw,Guid ProfileId  
) : IAppEvent
{
    public string Discriminator => "profile.created";
    public int Version => 1;
}