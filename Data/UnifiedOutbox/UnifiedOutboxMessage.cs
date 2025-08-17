namespace Data.UnifiedOutbox;

public partial class UnifiedOutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Discriminator { get; set; } // "profile.created"
    public int Version { get; set; } = 1;
    public required string PayloadJson { get; set; }   // JSON del IAppEvent
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    
    public Guid? TenantId { get; set; }
    public Guid? ActorId { get; set; }

    public DateTime? ProcessedOn { get; set; }
    public int Attempts { get; set; }
    public string? Error { get; set; }
}