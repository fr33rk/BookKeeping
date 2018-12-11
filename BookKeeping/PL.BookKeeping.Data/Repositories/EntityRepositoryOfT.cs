using PL.BookKeeping.Infrastructure.Data;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace PL.BookKeeping.Data.Repositories
{
    /// <summary>
    /// Generic implementation for all EntityFramework-based entity-repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityRepositoryOfT<TEntity> : IEntityRepositoryOfT<TEntity>
        where TEntity : class, new()
    {
        private readonly DbSet<TEntity> mDbSet;
        private readonly DbContext mContext;

        /// <summary>Initializes a new instance of the <see cref="EntityRepository{TEntity}"/> class.</summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public EntityRepositoryOfT(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            mDbSet = context.Set<TEntity>();

            if (mDbSet == null)
            {
                throw new NotSupportedException();
            }

            mContext = context;
        }

        /// <summary>
        /// Gets the default query for this repository.
        /// </summary>
        /// <returns>The default queryable for this repository.</returns>
        public virtual IQueryable<TEntity> GetQuery(bool readOnly = false)
        {
            if (readOnly)
                return mDbSet.AsNoTracking();
            else
                return mDbSet;
        }

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// <para />
        /// Note that this method uses the default query returned by <see creft="GetQuery(bool)" />.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return GetQuery().FirstOrDefault(predicate);
            }
            else
            {
                return GetQuery().FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// <para />
        /// Note that this method uses the default query returned by <see cref="GetQuery(bool)" />.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        /// <exception cref="InvalidOperationException">More than one element satisfies the condition in predicate.</exception>
        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return GetQuery().SingleOrDefault(predicate);
            }
            else
            {
                return GetQuery().SingleOrDefault();
            }
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public virtual void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            mDbSet.Add(entity);
        }

        /// <summary>
        /// Attaches the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Attach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            mDbSet.Attach(entity);
        }

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            mDbSet.Attach(entity);
            mDbSet.Remove(entity);
        }

        /// <summary>
        /// Deletes all entities that match the predicate.
        /// <para />
        /// Note that this method executes <see cref="GetQuery(bool)" /> to delete each entity.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var entities = mDbSet.Where(predicate);

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>Updates the specified entity.</summary>
        /// <param name="updated">The entity that is updated.</param>
        /// <param name="getKey">Function to get the key from entity.</param>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public virtual void Update(TEntity updated, Func<TEntity, int> getKey)
        {
            if (updated == null)
            {
                throw new ArgumentNullException("entity");
            }

            var entry = mContext.Entry<TEntity>(updated);

            if (entry.State == EntityState.Detached)
            {
                // Need the key to find the entity when detached.
                var attachedEntity = mDbSet.Find(getKey(updated));

                if (attachedEntity != null)
                {
                    var attachedEntry = mContext.Entry(attachedEntity);
                    // Copy values from updated entry to existing entity.
                    attachedEntry.CurrentValues.SetValues(updated);
                }
                else
                {
                    // This should attach the updated entity.
                    entry.State = EntityState.Modified;
                }
            }
        }

        /// <summary>Updates the specified updated.</summary>
        /// <param name="updated">The updated.</param>
        /// <param name="getKey">The get key.</param>
        /// <param name="mapping">The mapping.</param>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public virtual void Update(TEntity updated, Func<TEntity, int> getKey, Expression<Func<IUpdateConfiguration<TEntity>, object>> mapping)
        {
            if (updated == null)
            {
                throw new ArgumentNullException("entity");
            }

            var entry = mContext.Entry<TEntity>(updated);

            if (entry.State == EntityState.Detached)
            {
                // Need the key to find the entity when detached.
                var attachedEntity = mDbSet.Find(getKey(updated));

                if (attachedEntity != null)
                {
                    var attachedEntry = mContext.Entry(attachedEntity);
                    // Copy values from updated entry to existing entity.
                    //attachedEntry.CurrentValues.SetValues(updated);
                    mContext.UpdateGraph(updated, mapping);
                }
                else
                {
                    // This should attach the updated entity.
                    entry.State = EntityState.Modified;
                }
            }
        }

        /// <summary>
        /// Finds entities based on the provided criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Enumerable of all matching entities.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return GetQuery().Where(predicate);
        }

        /// <summary>
        /// Gets all entities available in the repository.
        /// <para />
        /// Note that this method executes the default query returned by <see cref="GetQuery(bool)" />.
        /// </summary>
        /// <returns>Enumerable of all entities.</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetQuery().AsQueryable();
        }

        /// <summary>
        /// Counts entities with the specified criteria.
        /// <para />
        /// Note that this method uses the default query returned by <see cref="GetQuery(bool)" />.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of entities that match the criteria.</returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return GetQuery().Count(predicate);
            }
            else
            {
                return GetQuery().Count();
            }
        }

        /// <summary>Executes a stored procedure which yields not results.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters passed to the procedure.</param>
        /// <returns>
        /// The result of the database engine.
        /// </returns>
        public int ExecuteProcedure(string procedureName, params object[] parameters)
        {
            var paramList = new List<string>();

            // Create a list of @px values
            for (var i = 0; i < parameters.Length; i++)
            {
                paramList.Add($"@P{i}");
            }

            string command;

            command = parameters.Length > 0 
				? $"CALL {procedureName} ({string.Join(",", paramList)})" 
				: $"CALL {procedureName}";

            return mContext.Database.ExecuteSqlCommand(command, parameters);            
        }

        /// <summary>Queries a stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters passed to the procedure.</param>
        /// <returns>
        /// The list of found results.
        /// </returns>
        public IEnumerable<TEntity> QueryProcedure(string procedureName, params object[] parameters)
        {
            var paramList = new List<string>();

            // Create a list of @px values
            for (var i = 0; i < parameters.Count(); i++)
            {
                paramList.Add($"@P{i}");
            }

            var command = $"EXECUTE PROCEDURE {procedureName} ({string.Join(",", paramList)})";

            return mContext.Database.SqlQuery<TEntity>(command, parameters);
        }
    }
}