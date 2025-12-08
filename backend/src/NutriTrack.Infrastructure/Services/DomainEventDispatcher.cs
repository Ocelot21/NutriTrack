using MediatR;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Notifications;
using NutriTrack.Domain.Common.Events;

namespace NutriTrack.Infrastructure.Services;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notification = (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()),
                domainEvent)!;

            await _mediator.Publish(notification, cancellationToken);
        }
    }
}
