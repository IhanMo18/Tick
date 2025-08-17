namespace Models.Blockade;

public sealed class Blockade
{
    public Guid Id { get; set; }
    public Guid BlockerProfileId{ get; set; }
    public Guid BlockedProfileId { get; set; }
    public Reason Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}