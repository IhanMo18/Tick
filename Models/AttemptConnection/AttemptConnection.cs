namespace Models.AttemptConnection;

public sealed class AttemptConnection
{
    public Guid Id { get; set; }
    public Guid FromProfileId { get; set; }
    public Guid ToProfileId { get; set; }
    public ConnectionStatus Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
}

public enum ConnectionStatus
{
    Pending = 1,
    Canceled = 2,
    Accepted = 3,
}