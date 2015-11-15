using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    /// <summary>Interface to the base traceable object data services.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IBaseTraceableObjectDataService<TEntity>
        where TEntity : BaseTraceableObject
    {
        /// <summary>Add a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity, that needs to be added.</param>
        void Add(TEntity entity);

        /// <summary>Update an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(TEntity entity);

        /// <summary>Tries to find an entity by its key. Will return 'null' when the entity was not found.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="complete">if set to <c>true</c> [Complete the found entity with all referred entities].</param>
        /// <returns></returns>
        TEntity GetByKey(int key, bool complete = false);

        /// <summary>Gets all.</summary>
        /// <returns></returns>
        IList<TEntity> GetAll(bool complete = false);

        /// <summary>When the entity, which is added to the database, contains references to other objects then
        /// these objects need to be reloaded within the lifetime of the unitOfWork so that Entity Framework knows
        /// which items need to be updated or added.
        /// </summary>
        /// <param name="unitOfWork">The unit of work gives access to the repositories.</param>
        /// <param name="entity">The entity for which the referenced objects need to be attached.</param>
        /// <returns>
        /// The same entity with the newly attached referenced objects.
        /// </returns>
        TEntity AttachEntities(IUnitOfWork unitOfWork, TEntity entity);
    }
}
