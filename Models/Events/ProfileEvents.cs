using System.Text.Json;
using Models.Profiles;
using Models.Shared;
using Prefs = Models.Profiles.Preferences; 

namespace Models.Events;

public record ProfileCreated(Guid OwnerId,Guid ProfileId,
string AliasRaw,
DateBirth Dob,
ApproximateCity City,
string? Bio,
Prefs Preferences,        
bool ShowCity,
bool HideOnline,
string? AvatarRoot) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string Payload => JsonSerializer.Serialize(new
    {
        ProfileId,
        AliasRaw,
        Dob.Value,
        City,
        Preferences.Types,
        ShowCity,
        HideOnline,
        AvatarRoot,
        Bio
    });
}
public record BioActualizada(Guid PerfilId, DateTime OccurredOn,string? Bio) : IDomainEvent
{
    public string Payload => JsonSerializer.Serialize(new
    {
        PerfilId,
        Bio
    });
}


public record ProfileStatusChanged(Guid ProfileId, ProfileStatus OldStatus, ProfileStatus NewStatus, string? Reason)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string Payload => JsonSerializer.Serialize(new
    {
        ProfileId,
        OldStatus,
        NewStatus,
        Reason,
        OccurredOn
    });
}

public record PreferencesUpdated(Guid PerfilId, DateTime OccurredOn) : IDomainEvent
{
    public string Payload => JsonSerializer.Serialize(new
    {
        PerfilId
        // agrega aquí campos si luego quieres detallar las preferences
    });
}

