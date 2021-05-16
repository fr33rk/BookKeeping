// <copyright file="IFullTraceableObjectDataService.cs" company="Delta Instruments">Copyright © Delta Instruments B.V.</copyright>

using System;
using System.Collections.Generic;
using PL.BookKeeping.Entities.Traceability;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    /// <summary>Interface to the full traceable object data services.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IFullTraceableObjectDataService<TEntity> : IBaseTraceableObjectDataService<TEntity>
       where TEntity : FullTraceableObject
    {
        ///// <summary>Deletes the specified entity. Note that traceable objects are never physically deleted from
        ///// the database, instead they are marked deleted.
        ///// </summary>
        ///// <param name="entity">The entity that should be marked deleted.</param>
        //void Delete(TEntity entity);

        /// <summary>Get the active traceable entity that has the given id.
        /// </summary>
        /// <param name="id">The identifier to search for.</param>
        /// <param name="complete">if set to <c>true</c> [All referring objects should be loaded with the entity].</param>
        /// <returns>The found entity, will be 'null' when nothing has been found.</returns>
        TEntity GetActiveById(int id, bool complete = false);

        /// <summary>Get a list of all active entities. (State = Active).
        /// </summary>
        /// <param name="complete">if set to <c>true</c> [All referring objects should be loaded with the entity].</param>
        /// <returns></returns>
        IList<TEntity> GetActiveEntities(bool complete = false);

        /// <summary>Get all FullTraceableObject entities that have a specific state.
        /// </summary>
        /// <param name="state">The state to search for.</param>
        /// <param name="complete">if set to <c>true</c> [All referring objects should be loaded with the found entities].</param>
        /// <returns></returns>
        IList<TEntity> GetByState(FullTraceableObject.ObjectState state, bool complete = false);

        /// <summary>Get the complete history of a specific entity.
        /// </summary>
        /// <param name="id">The identifier to search for.</param>
        /// <param name="complete">if set to <c>true</c> [All referring objects should be loaded with the found entities].</param>
        /// <returns></returns>
        IList<TEntity> GetHistory(int id, bool complete = false);
    }
}