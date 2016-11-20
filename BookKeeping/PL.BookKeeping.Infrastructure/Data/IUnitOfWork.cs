using System;

namespace PL.BookKeeping.Infrastructure.Data
{
    /// <summary>
    /// Interface defining a unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        bool SaveChanges();

        /// <summary>
        /// Gets the repository for the specified type that is created specifically for this unit of work.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity for which to get the repository.</typeparam>
        /// <returns>The entity repository.</returns>
        /// <exception cref="NotSupportedException">A repository for the specified type cannot be found.</exception>
        IEntityRepositoryOfT<TEntity> GetRepository<TEntity>()
            where TEntity : class, new();
    }
}