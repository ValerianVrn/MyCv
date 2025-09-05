using MediatR;
using MyCv.Rating.Application.ResultHandling;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Application.Commands
{
    /// <summary>
    /// Create assessment command handler.
    /// </summary>
    internal class CreateAssessmentCommandHandler(IAssessmentRepository assessmentRepository) : IRequestHandler<CreateAssessmentCommand, Result>
    {
        /// <summary>
        /// Assessment repository.
        /// </summary>
        private readonly IAssessmentRepository _assessmentRepository = assessmentRepository ?? throw new ArgumentNullException(nameof(assessmentRepository));

        /// <inheritdoc/>
        public async Task<Result> Handle(CreateAssessmentCommand command, CancellationToken cancellationToken)
        {
            // Check if the visitor has already an asessment.
            if (await _assessmentRepository.GetAsync(command.VisitorId, cancellationToken) is not null)
            {
                return Result.Failure(ResultErrors.AlreadyExists(command.VisitorId));
            }

            // Create the assessment.
            var assessment = new Assessment(
                entityGuid: Guid.NewGuid(),
                visitorId: command.VisitorId,
                score: command.Score,
                recommend: command.Recommend);

            // Save the entity.
            _ = _assessmentRepository.Create(assessment);

            if (!await _assessmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                return Result.Failure(ResultErrors.SaveEntitiesError(nameof(CreateAssessmentCommand)));
            }

            return Result.Success();
        }
    }
}
