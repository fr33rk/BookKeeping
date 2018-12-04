using System;
using System.Collections.Generic;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class EntryDataService : BaseTraceableObjectDataServiceOfT<Entry>, IEntryDataService
	{
		#region Fields

		private readonly Lazy<IPeriodDataService> mPeriodDataService;
		private readonly IEntryPeriodDataService mEntryPeriodDataService;

		#endregion Fields

		#region Constructor(s)

		public EntryDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService,
			Lazy<IPeriodDataService> periodDataService, IEntryPeriodDataService entryPeriodDataService)
			: base(uowFactory, authorizationService)
		{
			mPeriodDataService = periodDataService;
			mEntryPeriodDataService = entryPeriodDataService;
		}

		#endregion Constructor(s)

		public IList<Entry> GetRootEntries()
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var root = GetAll()
					.Where(e => e.IsActive && e.ParentEntry == null);

				return root.ToList();
			}
		}

		public IList<Entry> Get3rdLevelEntries()
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				var root = GetAll()
					.Join(GetAll(), e => e.ParentEntryKey, e2 => e2.Key, (e, e2) => new { Entry = e, Entry2 = e2 })
					.Join(GetAll(), e => e.Entry2.ParentEntryKey, e3 => e3.Key, (e, e3) => new { Entry = e.Entry })
					.Select(e => e.Entry);

				return root.ToList();
			}
		}

		public override Entry AttachEntities(IUnitOfWork unitOfWork, Entry entity)
		{
			if (entity.ParentEntry != null)
			{
				var entries = unitOfWork.GetRepository<Entry>();
				entity.ParentEntry = entries.FirstOrDefault(e => e.Key == entity.ParentEntry.Key);
			}

			return base.AttachEntities(unitOfWork, entity);
		}

		public override bool Add(Entry entity)
		{
			// First add the entry to the database.
			var result = base.Add(entity);

			// Then create a EntryPeriod for each period in the database.
			var periods = mPeriodDataService.Value.GetAll();

			EntryPeriod newEntryPeriod;

			foreach (var period in periods)
			{
				newEntryPeriod = new EntryPeriod();
				newEntryPeriod.Entry = entity;
				newEntryPeriod.Period = period;
				mEntryPeriodDataService.Add(newEntryPeriod);
			}

			return result;
		}

		public bool Delete(Entry entity)
		{
			var lastYearTheEntryWasUsed = GetLastYearTheEntryWasUsed(entity);
			// Never? Just delete the entry
			if (lastYearTheEntryWasUsed == null)
			{
				return base.Delete(entity);
			}
			
			// Otherwise mark it inactive
			entity.IsActive = false;
			Update(entity);

			// Remove the entry from all years after the last year it was used.
			mEntryPeriodDataService.Delete(entity, lastYearTheEntryWasUsed.Value);

			return true;
		}

		private int? GetLastYearTheEntryWasUsed(Entry entity)
		{
			using (var uow = mUOWFactory.Create())
			{
				return uow.GetRepository<Entry>()
					.GetQuery(true)
					.Where(e => e.Key == entity.Key)
					.Join(uow.GetRepository<EntryPeriod>().GetAll(), e => e.Key, ep => ep.EntryKey,
						((e, ep) => new { entity = e, entityPeriod = ep }))
					.Join(uow.GetRepository<Period>().GetAll(), ep => ep.entityPeriod.PeriodKey, p => p.Key,
						(ep, p) => new { period = p, entityPeriod = ep.entityPeriod })
					.Join(uow.GetRepository<Transaction>().GetAll(), pep => pep.entityPeriod.Key, t => t.EntryPeriodKey,
						(pep, t) => new { period = pep.period })
					.Max(p => p.period.Year);
			}
		}
	}
}