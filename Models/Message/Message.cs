namespace Models.Message;

public sealed class Mensaje
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid SenderProfileId{ get; set; }
    public string Content{ get; set; } = default!;
    public string? MediaRef { get; set; }            // si adjuntas
    public string Status { get; set; } = "visible";  // visible/moderado/eliminado
    public DateTimeOffset SendedIn { get; set; }
    public DateTimeOffset? ReadIn{ get; set; }
}