using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class EntryPeriodDataService : BaseTraceableObjectDataServiceOfT<EntryPeriod>, IEntryPeriodDataService
	{
		#region Constructor(s)

		public EntryPeriodDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
			: base(uowFactory, authorizationService)
		{
		}

		#endregion Constructor(s)

		public override EntryPeriod AttachEntities(IUnitOfWork unitOfWork, EntryPeriod entity)
		{
			if (entity.Period != null)
			{
				var periods = unitOfWork.GetRepository<Period>();
				entity.Period = periods.FirstOrDefault(e => e.Key == entity.Period.Key);
			}
			if (entity.Entry != null)
			{
				var entries = unitOfWork.GetRepository<Entry>();
				entity.Entry = entries.FirstOrDefault(e => e.Key == entity.Entry.Key);
			}
			return base.AttachEntities(unitOfWork, entity);
		}

		/// <summary>Add include statements to 'query' so that referred objects are loaded
		/// together with the entity in one query to the database.
		/// </summary>
		/// <param name="query">The query where the include statements need to be added to.</param>
		/// <returns></returns>
		public override IQueryable<EntryPeriod> CompleteQry(IQueryable<EntryPeriod> query)
		{
			return base.CompleteQry(query.Include(e => e.Period)
										 .Include(e => e.Entry));
		}

		#region IEntryPeriodDataService

		public IList<EntryPeriod> GetByEntryAndYear(Entry entry, int year)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<EntryPeriod>();
				var qry = repository.GetQuery()
					.Where(ep => (ep.Entry.Key == entry.Key) && (ep.Period.PeriodStart.Year == year));

				qry = CompleteQry(qry);

				return qry.ToList();
			}
		}

		public IList<EntryPeriod> GetByYear(int year)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<EntryPeriod>();
				var qry = repository.GetQuery()
					.Where(ep => ep.Period.PeriodStart.Year == year);

				qry = CompleteQry(qry);

				return qry.ToList();
			}
		}

		public void ReCalculateTotalAmounts(DateTime periodStart, DateTime periodEnd)
		{
			// Next update the priorities
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<EntryPeriod>();

				// Move all lower priority rules one priority down.
				repository.ExecuteProcedure("RECALC_AMOUNTS", periodEnd, periodStart);
			}
		}

		public void Delete(Entry ofEntry, int afterYear)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var repository = unitOfWork.GetRepository<EntryPeriod>();
				var afterDate = new DateTime(afterYear, 12, 31)
					.AddDays(1)
					.AddTicks(-1);
				repository.Delete(ep => ep.Entry.Key == ofEntry.Key && ep.Period.PeriodStart >= afterDate);

				unitOfWork.SaveChanges();
			}
		}

		#endregion IEntryPeriodDataService
	}
}