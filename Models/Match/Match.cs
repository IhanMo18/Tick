namespace Models.Match;

public sealed class Match
{
    public Guid Id { get; set; }
    public Guid ProfileAId { get; set; }
    public Guid ProfileBId { get; set; }
    public Guid? CreatedFromIntentId{ get; set; }
    public MathcStatus Status;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}


public enum MathcStatus
{
    Blocked = 1,
    Cancelled = 2,
    Active = 3,
}