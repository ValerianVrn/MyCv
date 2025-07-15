using MediatR;

namespace MyCv.Common.Domain.EventSourcing
{
    /// <summary>
    /// Domain base object.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Domain events.
        /// </summary>
        private readonly List<INotification> _domainEvents = [];

        /// <summary>
        /// GUID of the entity.
        /// </summary>
        public Guid EntityGuid { get; protected set; }

        /// <summary>
        /// Domain events.
        /// </summary>
        public IReadOnlyCollection<INotification>? DomainEvents
        {
            get
            {
                return _domainEvents.AsReadOnly();
            }
        }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityGuid"></param>
        protected Entity(Guid entityGuid)
        {
            if (entityGuid.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(entityGuid), $"The GUID of the {GetType().Name} entity can not be empty.");
            }

            EntityGuid = entityGuid;
            _domainEvents = [];
        }

        /// <summary>
        /// Add a domain event.
        /// </summary>
        /// <param name="eventItem"></param>
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Remove a domain event.
        /// </summary>
        /// <param name="eventItem"></param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            _ = _domainEvents.Remove(eventItem);
        }

        /// <summary>
        /// Remove all domain events.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Check if two entities are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var isNotEntity = obj is not Entity;

            if (obj == null || isNotEntity)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var item = (Entity)obj;

            return item.EntityGuid == EntityGuid;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        /// <summary>
        /// Not equal operator/
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
