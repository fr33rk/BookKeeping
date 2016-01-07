using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PL.BookKeeping.Infrastructure.Data
{
    /// <summary>
    /// Interface for all entity-repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntityRepositoryOfT<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the default query for this repository.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> the repository is read-only. No tracking is being done by Entity Framework. Use this when getting info from database.</param>
        /// <returns>The default queryable for this repository.</returns>
        IQueryable<TEntity> GetQuery(bool readOnly = false);

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        /// <exception cref="InvalidOperationException">More than one element satisfies the condition in predicate.</exception>
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        void Add(TEntity entity);

        /// <summary>
        /// Attaches the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        void Attach(TEntity entity);

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all entities that match the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>Updates the specified entity.</summary>
        /// <param name="updated">The updated entity.</param>
        /// <param name="getKey">Function to get the key from given entity.</param>
        /// <returns></returns>
        void Update(TEntity updated, Func<TEntity, int> getKey);

        /// <summary>Updates the specified updated.</summary>
        /// <param name="updated">The updated.</param>
        /// <param name="getKey">The get key.</param>
        /// <param name="mapping">The mapping.</param>
        void Update(TEntity updated, Func<TEntity, int> getKey, Expression<Func<IUpdateConfiguration<TEntity>, object>> mapping);

        /// <summary>
        /// Finds entities based on provided criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Enumerable of all matching entities.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>Gets all entities available in the repository.</summary>
        /// Note that this method executes the default query returned by GetQuery()
        /// <returns>Enumerable of all entities.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Counts entities with the specified criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of entities that match the criteria.</returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);
    }
}