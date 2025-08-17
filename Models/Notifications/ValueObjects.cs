using System.Text.Json;

namespace Models.Notification;

public readonly record struct NotificationType
{
    public string Value { get; }
    private NotificationType(string value) => Value = value;

    // Instancias conocidas
    public static readonly NotificationType Match   = new("match");
    public static readonly NotificationType Message = new("message");
    public static readonly NotificationType System  = new("system");

    public static IReadOnlyList<NotificationType> All => new[] { Match, Message, System };

    public static NotificationType Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Tipo de notificación requerido.");

        // Normaliza y mapea a las instancias conocidas
        return value.Trim().ToLowerInvariant() switch
        {
            "match"   => Match,
            "message" => Message,
            "system"  => System,
            _ => throw new ArgumentException(
                $"Tipo '{value}' no permitido. Usa: {string.Join(", ", All.Select(t => t.Value))}.")
        };
    }
    
    public static bool TryCreate(string? value, out NotificationType type)
    {
        type = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        switch (value.Trim().ToLowerInvariant())
        {
            case "match":   type = Match;   return true;
            case "message": type = Message; return true;
            case "system":  type = System;  return true;
            default: return false;
        }
    }

    public override string ToString() => Value;
}



public readonly record struct NotificationData
{
    public string Json { get; }
    private NotificationData(string json) => Json = json;
    
    public static NotificationData FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON requerido.");
        using var _ = JsonDocument.Parse(json);
        return new NotificationData(json);
    }
    
    public static NotificationData FromObject(object data)
        => new(JsonSerializer.Serialize(data));
}

