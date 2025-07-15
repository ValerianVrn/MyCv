using MediatR;

namespace MyCv.Common.Domain.EventSourcing
{
    /// <summary>
    /// Root (or primary) entity.
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Load the aggregate using domain events (event sourcing).
        /// </summary>
        /// <param name="events"></param>
        void Rehydrate(IEnumerable<INotification> events);
    }
}
