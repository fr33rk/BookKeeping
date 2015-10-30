namespace PL.BookKeeping.Infrastructure.Data
{
    /// <summary>
    /// Interface for creating a unit of work.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Initializes this unit-of-work factory.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>The unit of work.</returns>
        IUnitOfWork Create();
    }
}
