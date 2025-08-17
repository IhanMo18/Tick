namespace Models.Interest;

public sealed class Interest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Category { get; set; }         // opcional
    public bool Approved { get; set; }             // moderación
    public bool Sensitive { get; set; }             // para filtros
    public int Popularity { get; set; }           // #perfiles asociados
    public DateTimeOffset CreatedAt { get; set; }
}