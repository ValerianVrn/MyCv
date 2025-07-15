using MediatR;

namespace MyCv.Common.Domain.EventSourcing
{
    /// <summary>
    /// Abstract aggregate root.
    /// </summary>
    /// <remarks>
    /// Constructor with GUID.
    /// </remarks>
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        /// <summary>
        /// Version of the aggregate (event sourcing).
        /// </summary>
        public long Version { get; private set; } = -1;

        /// <param name="aggregateGuid"></param>
        public AggregateRoot(Guid aggregateGuid) : base(aggregateGuid)
        {
        }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        protected AggregateRoot() : base()
        {
        }

        /// <summary>
        /// Load the entity by applying domain events on it.
        /// </summary>
        /// <param name="history"></param>
        public void Rehydrate(IEnumerable<INotification> events)
        {
            foreach (var e in events)
            {
                Apply(e);
                Version++;
            }
        }

        /// <summary>
        /// Apply an event to the entity.
        /// </summary>
        /// <param name="event"></param>
        protected abstract void Apply(INotification @event);
    }
}
