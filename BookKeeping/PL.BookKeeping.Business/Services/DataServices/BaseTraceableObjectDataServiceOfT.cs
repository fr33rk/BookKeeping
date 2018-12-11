using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class BaseTraceableObjectDataServiceOfT<TEntity> : IBaseTraceableObjectDataService<TEntity>
		where TEntity : BaseTraceableObject, new()
	{
		#region Definitions

		/// <summary>The unit of work factor to use for accessing the repositories.
		/// </summary>
		protected IUnitOfWorkFactory mUOWFactory;

		private readonly IAuthorizationService mAuthorizationService;

		#endregion Definitions

		#region Constructor(s)

		/// <summary>Initializes a new instance of the <see cref="BaseTraceableObjectDataService{TEntity}"/> class.</summary>
		/// <param name="uowFactory">The uow factory.</param>
		/// <param name="authorizationService">The authorization service.</param>
		public BaseTraceableObjectDataServiceOfT(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
		{
			mUOWFactory = uowFactory;
			mAuthorizationService = authorizationService;
		}

		#endregion Constructor(s)

		#region IBaseTraceableObjectDataService

		/// <summary>Add a new entity to the database.
		/// </summary>
		/// <param name="entity">The entity, that needs to be added.</param>
		public virtual bool Add(TEntity entity)
		{
			entity = Mark(entity);

			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<TEntity>();

				entity = AttachEntities(unitOfWork, entity);

				repository.Add(entity);
				return unitOfWork.SaveChanges();
			}
		}

		/// <summary>Update an existing entity in the database.
		/// </summary>
		/// <param name="entity">The entity that needs to be updated.</param>
		public virtual bool Update(TEntity entity)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<TEntity>();

				entity = AttachEntities(unitOfWork, entity);

				repository.Update(entity, e => e.Key);
				return unitOfWork.SaveChanges();
			}
		}

		/// <summary>Deletes the specified entity from the database.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public virtual bool Delete(TEntity entity)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<TEntity>();

				repository.Delete(e => e.Key == entity.Key);

				return unitOfWork.SaveChanges();
			}
		}

		/// <summary>Tries to find an entity by its key. Will return 'null' when the entity was not found.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <param name="complete">if set to <c>true</c> [Complete the found entity with all referred entities].</param>
		/// <returns></returns>
		public TEntity GetByKey(int key, bool complete = false)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<TEntity>();

				if (complete)
				{
					var qry = repository.GetQuery()
						.Where(e => e.Key == key);

					qry = CompleteQry(qry);

					return qry.SingleOrDefault();
				}
				else
				{
					return repository.SingleOrDefault(e => e.Key == key);
				}
			}
		}

		/// <summary>Gets all.</summary>
		/// <returns></returns>
		public IList<TEntity> GetAll(bool complete = false)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<TEntity>();
				if (!complete)
				{
					return repository.GetAll().ToList();
				}
				else
				{
					var qry = CompleteQry(repository.GetQuery());
					return qry.ToList();
				}
			}
		}

		#endregion IBaseTraceableObjectDataService

		#region Completion and attaching

		/// <summary>Add include statements to 'query' so that referred objects are loaded
		/// together with the entity in one query to the database.
		/// </summary>
		/// <param name="query">The query where the include statements need to be added to.</param>
		/// <returns></returns>
		public virtual IQueryable<TEntity> CompleteQry(IQueryable<TEntity> query)
		{
			return query.Include(e => e.Creator);
		}

		/// <summary>When the entity, which is added to the database, contains references to other objects then
		/// these objects need to be reloaded within the lifetime of the unitOfWork so that Entity Framework knows
		/// which items need to be updated or added.
		/// </summary>
		/// <param name="unitOfWork">The unit of work gives access to the repositories.</param>
		/// <param name="entity">The entity for which the referenced objects need to be attached.</param>
		/// <returns>The same entity with the newly attached referenced objects.</returns>
		public virtual TEntity AttachEntities(IUnitOfWork unitOfWork, TEntity entity)
		{
			if (entity.Creator != null)
			{
				var users = unitOfWork.GetRepository<User>();
				entity.Creator = users.FirstOrDefault(e => e.Key == entity.Creator.Key);
			}
			return entity;
		}

		#endregion Completion and attaching

		#region Helper methods

		/// <summary>Update the creation date and time and the creator with the current values.
		/// </summary>
		/// <param name="entity">The entity that needs to be marked.</param>
		/// <returns>The marked entity.</returns>
		public TEntity Mark(TEntity entity)
		{
			entity.CreationDT = DateTime.Now;
			entity.Creator = mAuthorizationService.CurrentUser;

			return entity;
		}

		#endregion Helper methods
	}
}