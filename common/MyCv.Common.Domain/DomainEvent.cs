using MediatR;

namespace MyCv.Common.Domain
{
    /// <summary>
    /// Represents a base domain event.
    /// </summary>
    /// <param name="aggregateGuid"></param>
    /// <param name="aggregateType"></param>
    /// <param name="eventType"></param>
    public abstract class DomainEvent(Guid aggregateGuid, string aggregateType, string eventType) : INotification
    {
        /// <summary>
        /// GUID of the aggregate on which the event applies.
        /// </summary>
        public Guid AggregateGuid { get; } = aggregateGuid;

        /// <summary>
        /// Gets the type of the aggregate.
        /// </summary>
        public string AggregateType { get; } = aggregateType;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        public string EventType { get; } = eventType;

        /// <summary>
        /// Gets the date and time when the event occurred.
        /// </summary>
        public DateTime OccuredOn { get; } = DateTime.Now;
    }
}
