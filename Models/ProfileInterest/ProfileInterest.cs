namespace Models.ProfileInterest;

public sealed class PerfilInteres
{
    public Guid ProfileId { get; set; }
    public Guid InterestId { get; set; }
    public DateTimeOffset AddedAt { get; set; }
}