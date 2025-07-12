using MyCv.Common.Domain.Cqrs;

namespace MyCv.Rating.Domain.AggregateModels.RatingAggregate
{
    /// <summary>
    /// Rating root entity.
    /// </summary>
    public class Rating : Entity, IAggregateRoot
    {
        /// <summary>
        /// ID of the visitor who posts the rating.
        /// </summary>
        public string VisitorId { get; }

        /// <summary>
        /// Value of rating given by the visitor.
        /// </summary>
        public int RatingValue { get; }

        /// <summary>
        /// Indicate if the visitor also gives recommendation.
        /// </summary>
        public bool Recommend { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityGuid">GUID of the rating.</param>
        /// <param name="visitorId">ID of the visitor who posts the rating</param>
        /// <param name="ratingValue">Value of rating given by the visitor.</param>
        /// <param name="recommend">Indicate if the visitor also gives recommendation</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Rating(Guid entityGuid, string visitorId, int ratingValue, bool recommend) : base(entityGuid)
        {
            // Check that visitor ID is not null.
            if (string.IsNullOrEmpty(visitorId))
            {
                throw new ArgumentNullException(nameof(visitorId), "The visitor ID must not be null or empty.");
            }

            // Check that the rating value is between 1 and 5.
            if (ratingValue is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(ratingValue), ratingValue, $"The rating value must be between 1 and 5.");
            }

            VisitorId = visitorId;
            RatingValue = ratingValue;
            Recommend = recommend;
        }
    }
}
