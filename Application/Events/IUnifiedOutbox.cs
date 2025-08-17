
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Application.Events;

public interface IUnifiedOutbox
{
    Task EnqueueAsync(IAppEvent evt, string? correlationId, Guid? tenantId, Guid? actorId, CancellationToken ct);
}

