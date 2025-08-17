using System.Text.Json;
using Application.Events;
using Data.Context;

using Data.UnifiedOutbox;

namespace Data.Events;

public sealed class UnifiedOutbox : IUnifiedOutbox
{
    private readonly AppDbContext _db;
    private readonly IEventTypeRegistry _registry;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public UnifiedOutbox(AppDbContext db, IEventTypeRegistry registry)
    {
        _db = db;
        _registry = registry;
    }

    public Task EnqueueAsync(IAppEvent evt, string? corr, Guid? tenant, Guid? actor, CancellationToken ct)
    {
        var (disc, ver) = _registry.GetInfo(evt.GetType());
        var row = new UnifiedOutboxMessage
        {
            Discriminator = disc,
            Version = ver,
            PayloadJson = JsonSerializer.Serialize(evt, evt.GetType(), _json),
            OccurredOn = evt.OccurredOn,
            ActorId = actor
        };

        _db.Set<UnifiedOutboxMessage>().Add(row);
        return Task.CompletedTask;
    }
}