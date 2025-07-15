using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCv.Common.Domain;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure.EntityTypeConfigurations;
using MyCv.Rating.Infrastructure.Extensions;

namespace MyCv.Rating.Infrastructure
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="mediator"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public class RatingContext(DbContextOptions<RatingContext> options, IMediator mediator) : DbContext(options), IUnitOfWork
    {
        /// <summary>
        /// Current transaction.
        /// </summary>
        private IDbContextTransaction? _currentTransaction;

        /// <summary>
        /// Returns true if a transaction is active, false otherwise.
        /// </summary>
        public bool HasActiveTransaction
        {
            get
            {
                return _currentTransaction is not null;
            }
        }

        /// <summary>
        /// Assessment table.
        /// </summary>
        public DbSet<Assessment> Assessments { get; set; }

        /// <summary>
        /// Mediator.
        /// </summary>
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Fields-to-database mapping configuration.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.ApplyConfiguration(new AssessmentEntityTypeConfiguration());
        }

        /// <summary>
        /// Begins and returns a new transaction.
        /// </summary>
        /// <remarks>
        /// Returns any active transaction.
        /// </remarks>
        /// <returns></returns>
        public async Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_currentTransaction != null)
            {
                return null;
            }

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

            return _currentTransaction;
        }

        /// <summary>
        /// Commits the active transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            if (transaction != _currentTransaction)
            {
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
            }

            try
            {
                _ = await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (HasActiveTransaction)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Rollback the active transaction.
        /// </summary>
        /// <returns></returns>
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (HasActiveTransaction)
                {
                    _currentTransaction?.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Save entities in database asynchronously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection.  
            await _mediator.DispatchDomainEventsAsync(this);

            // Commit all the changes (from the Command Handler and Domain Event Handlers) performed through the DbContext.
            _ = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
