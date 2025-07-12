using MyCv.Common.Domain.Cqrs;

namespace MyCv.Rating.Domain.AggregateModels.AssessmentAggregate
{
    /// <summary>
    /// Assessment root entity.
    /// </summary>
    public class Assessment : Entity, IAggregateRoot
    {
        /// <summary>
        /// ID of the visitor who posts the assessment.
        /// </summary>
        public string VisitorId { get; }

        /// <summary>
        /// Score given by the visitor.
        /// </summary>
        public int Score { get; }

        /// <summary>
        /// Indicate if the visitor also gives recommendation.
        /// </summary>
        public bool Recommend { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityGuid">GUID of the assessment.</param>
        /// <param name="visitorId">ID of the visitor who posts the assessment</param>
        /// <param name="score">Score given by the visitor.</param>
        /// <param name="recommend">Indicate if the visitor also gives recommendation</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Assessment(Guid entityGuid, string visitorId, int score, bool recommend) : base(entityGuid)
        {
            // Check that visitor ID is not null.
            if (string.IsNullOrEmpty(visitorId))
            {
                throw new ArgumentNullException(nameof(visitorId), "The visitor ID must not be null or empty.");
            }

            // Check that the score is between 1 and 5.
            if (score is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(score), score, $"The score must be between 1 and 5.");
            }

            VisitorId = visitorId;
            Score = score;
            Recommend = recommend;
        }
    }
}
