using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Domain.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
