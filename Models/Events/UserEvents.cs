using System.Text.Json;
using Models.Profiles;
using Models.Shared;
using Models.Users;

namespace Models.Events;

public record UserCreated(Guid UserId,string Email, UserRole UserRole) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string Payload => JsonSerializer.Serialize(new
    {
        UserId,
        Email,
        UserRole,
        OccurredOn
    });
}

public record UserEmailConfirmed(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; }= DateTime.UtcNow;

    public string Payload => JsonSerializer.Serialize(new
    {
        UserId,
        OccurredOn
    });
}


public record UserPasswordChanged(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; }= DateTime.UtcNow;

    public string Payload => JsonSerializer.Serialize(new
    {
        UserId,
        OccurredOn
    });
}

public record UserLoggedIn(Guid UserId, DateTime LastAccessAt):IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    
    public string Payload => JsonSerializer.Serialize(new
    {
        UserId,
        LastAccessAt,
        OccurredOn
    });
}