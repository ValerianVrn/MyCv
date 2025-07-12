using Microsoft.EntityFrameworkCore;
using MyCv.Common.Domain;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Infrastructure.Repositories
{
    /// <summary>
    /// Assessment repository.
    /// </summary>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public class AssessmentRepository(RatingContext context) : IAssessmentRepository
    {
        /// <summary>
        /// Context.
        /// </summary>
        private readonly RatingContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Unit of work.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Assessment>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Assessments.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Assessment Create(Assessment assessment)
        {
            return _context.Assessments.Add(assessment).Entity;
        }
    }
}
