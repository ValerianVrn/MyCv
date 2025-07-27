using MediatR;
using MyCv.Common.Domain.Cqrs;

namespace MyCv.Rating.Infrastructure.Extensions
{
    /// <summary>
    /// Extension for mediator
    /// </summary>
    internal static class MediatorExtension
    {
        /// <summary>
        /// Dispatch domain events of the context asynchronously.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="ratingContext"></param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, RatingContext ratingContext)
        {
            var domainEntities = ratingContext.ChangeTracker
                                    .Entries<Entity>()
                                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0);

            var domainEvents = domainEntities
                               .SelectMany(x => x.Entity.DomainEvents)
                               .ToList();

            domainEntities.ToList()
                          .ForEach(entity => entity.Entity.ClearDomainEvents());

            // Publish domain events.
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
