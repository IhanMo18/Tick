using System.Text.Json;
using Application.Events;
using Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Data.UnifiedOutbox;

public sealed class UnifiedOutboxDispatcherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UnifiedOutboxDispatcherService> _logger;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    private const int MaxAttempts = 5;

    public UnifiedOutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<UnifiedOutboxDispatcherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var registry = scope.ServiceProvider.GetRequiredService<IEventTypeRegistry>();

                var batch = await db.Set<UnifiedOutboxMessage>()
                    .Where(x => x.ProcessedOn == null && x.Attempts < MaxAttempts)
                    .OrderBy(x => x.OccurredOn)
                    .Take(100)
                    .ToListAsync(ct);

                if (batch.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), ct);
                    continue;
                }

                foreach (var row in batch)
                {
                    try
                    {
                        if (!registry.TryResolve(row.Discriminator, row.Version, out var type) || type is null)
                        {
                            row.ProcessedOn = DateTime.UtcNow;
                            row.Error = $"Unknown event: {row.Discriminator} v{row.Version}";
                            continue;
                        }

                        var evt = (IAppEvent?)JsonSerializer.Deserialize(row.PayloadJson, type, _json);
                        if (evt is null)
                        {
                            row.Attempts++;
                            row.Error = "Null after deserialization.";
                            continue;
                        }

                        await mediator.Publish(evt, ct);

                        row.ProcessedOn = DateTime.UtcNow;
                        row.Error = null;
                    }
                    catch (Exception ex)
                    {
                        row.Attempts++;
                        row.Error = ex.Message.Length > 1000 ? ex.Message[..1000] : ex.Message;
                        _logger.LogError(ex, "Dispatch error {Id} ({Disc} v{Ver})", row.Id, row.Discriminator, row.Version);
                    }
                }

                await db.SaveChangesAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnifiedOutbox loop error");
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }
}