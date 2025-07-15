namespace MyCv.Common.Domain.Cqrs
{
    /// <summary>
    /// Repository interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Unit of work.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}
