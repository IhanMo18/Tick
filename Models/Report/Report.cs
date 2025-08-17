namespace Models.Report;

public sealed class Report
{
    public Guid Id { get; set; }
    public Guid ReporterProfileId { get; set; }
    public string ObjectiveType { get; set; } = default!; // "perfil" | "mensaje"
    public Guid TargetId { get; set; }
    public string Reason { get; set; } = default!;
    public string? Details { get; set; }
    public string Status { get; set; } = "abierto"; // abierto/en_revision/cerrado/rechazado
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SolvedIn { get; set; }
}