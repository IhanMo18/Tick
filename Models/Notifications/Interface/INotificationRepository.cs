using Models.Notification;

namespace Models.Notifications.Interface;

public interface INotificationRepository
{
    Task AddAsync(Notification.Notification notification, CancellationToken ct = default);
    
    Task<Notification.Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task UpdateAsync(Notification.Notification profile, CancellationToken ct = default);
    
    /// <summary>
    /// Devuelve true si existe una notificación con el mismo perfil, tipo
    /// y (opcional) el mismo payload JSON.
    /// </summary>
    Task<bool> ExistsAsync(Guid profileId, NotificationType type, string? payloadJson = null, CancellationToken ct = default);

    /// <summary>
    /// Overload práctico: pasas el objeto y lo serializa igual que NotificationData.
    /// </summary>
    Task<bool> ExistsAsync(Guid profileId, NotificationType type, object payload, CancellationToken ct = default);
}