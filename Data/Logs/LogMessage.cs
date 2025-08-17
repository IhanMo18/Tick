namespace Data.Logs;

public class LogMessage
{
    public Guid Id { get; set; }
    public required string Action { get; set; } 
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public DateTime OcurredOn { get; set; }
}