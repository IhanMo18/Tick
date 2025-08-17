using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Shared;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    string Payload { get; }
}

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _events = new();
    [NotMapped]
    public IReadOnlyList<IDomainEvent> DomainEvents => _events;
    protected void Raise(IDomainEvent @event) => _events.Add(@event);
    public void ClearDomainEvents() => _events.Clear();
}