using Data.Context;
using Microsoft.EntityFrameworkCore;
using Models.Notification;
using Models.Notifications.Interface;

namespace Data.Repository.Notification 
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Models.Notification.Notification> _repository;

        public NotificationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repository = _context.Set<Models.Notification.Notification>();
        }

        public async Task AddAsync(Models.Notification.Notification notification, CancellationToken ct = default)
        { 
            await _repository.AddAsync(notification, ct);
        }

        public async Task<Models.Notification.Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _repository.FindAsync(new object?[] { id }, ct);
        }

        public Task UpdateAsync(Models.Notification.Notification profile, CancellationToken ct = default)
        {
            _repository.Update(profile);
            return Task.CompletedTask;
        }
        
        public async Task<bool> ExistsAsync(Guid profileId, NotificationType type, string? payloadJson = null, CancellationToken ct = default)
        {
            return await _repository
                .AsNoTracking()
                .AnyAsync(n => n.ProfileId == profileId && n.Type == type, ct);
        }

        public Task<bool> ExistsAsync(Guid profileId, NotificationType type, object payload, CancellationToken ct = default)
        {
            var json = NotificationData.FromObject(payload).Json;
            return ExistsAsync(profileId, type, json, ct);
        }
    }
}
