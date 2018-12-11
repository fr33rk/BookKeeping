using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class ProcessingRuleDataService : BaseTraceableObjectDataServiceOfT<ProcessingRule>, IProcessingRuleDataService

	{
		public ProcessingRuleDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
			: base(uowFactory, authorizationService)
		{
		}

		public IList<ProcessingRule> GetAllSorted()
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<ProcessingRule>();
				return CompleteQry(repository.GetQuery(true))
					.OrderBy(pr => pr.Priority)
					.ToList();
			}
		}

		public override IQueryable<ProcessingRule> CompleteQry(IQueryable<ProcessingRule> query)
		{
			return base.CompleteQry(query.Include(e => e.Entry));
		}

		public IList<ProcessingRule> GetByEntry(Entry entry)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<ProcessingRule>();
				return CompleteQry(repository.GetQuery(true))
					.Where(r => r.EntryKey == entry.Key)
					.OrderBy(r => r.Priority)
					.ToList();
			}
		}

		public override bool Add(ProcessingRule entity)
		{
			// First add the entity to the database.
			//base.Add(entity);

			// Next update the priorities
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<ProcessingRule>();

				entity = AttachEntities(unitOfWork, entity);

				// Move all lower priority rules one priority down.
				repository.ExecuteProcedure("UPDATE_RULE_PRIORITY", entity.Priority);

				// Insert the new rule
				return base.Add(entity);
			}
		}

		/// <summary>When the entity, which is added to the database, contains references to other objects then
		/// these objects need to be reloaded within the lifetime of the unitOfWork so that Entity Framework knows
		/// which items need to be updated or added.
		/// </summary>
		/// <param name="unitOfWork">The unit of work gives access to the repositories.</param>
		/// <param name="entity">The entity for which the referenced objects need to be attached.</param>
		/// <returns>
		/// The same entity with the newly attached referenced objects.
		/// </returns>
		public override ProcessingRule AttachEntities(IUnitOfWork unitOfWork, ProcessingRule entity)
		{
			entity = base.AttachEntities(unitOfWork, entity);

			if (entity.Entry != null)
			{
				var entries = unitOfWork.GetRepository<Entry>();
				entity.Entry = entries.FirstOrDefault(e => e.Key == entity.Entry.Key);
				entity.EntryKey = entity.Entry.Key;
			}
			return entity;
		}

		public void SwapByPriority(ProcessingRule swapThis, ProcessingRule swapWithThat)
		{
			var priority = swapThis.Priority;

			swapThis.Priority = swapWithThat.Priority;

			// Priority needs to be unique in the database. So first set to -1 to avoid problems with this constraint.
			swapWithThat.Priority = -1;
			Update(swapWithThat);

			Update(swapThis);

			// Give the final priority
			swapWithThat.Priority = priority;
			Update(swapWithThat);
		}

		public void DeleteByEntry(Entry entry)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<ProcessingRule>();

				repository.Delete(e => e.EntryKey == entry.Key);

				unitOfWork.SaveChanges();
			}
		}
	}
}