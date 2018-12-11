using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class EntryDataService : BaseTraceableObjectDataServiceOfT<Entry>, IEntryDataService
	{
		#region Fields

		private readonly Lazy<IPeriodDataService> mPeriodDataService;
		private readonly IEntryPeriodDataService mEntryPeriodDataService;
		private readonly IProcessingRuleDataService mProcessingRuleDataService;

		#endregion Fields

		#region Constructor(s)

		public EntryDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService,
			Lazy<IPeriodDataService> periodDataService, IEntryPeriodDataService entryPeriodDataService,
			IProcessingRuleDataService processingRuleDataService)
			: base(uowFactory, authorizationService)
		{
			mPeriodDataService = periodDataService;
			mEntryPeriodDataService = entryPeriodDataService;
			mProcessingRuleDataService = processingRuleDataService;
		}

		#endregion Constructor(s)

		public IList<Entry> GetRootEntriesOfYear(int year)
		{
			using (var unitOfWork = mUOWFactory.Create())
			{
				//var allActiveInYear = unitOfWork.GetRepository<Entry>()
				//	.GetQuery(true)
				//	.Join(unitOfWork.GetRepository<EntryPeriod>().GetAll(),
				//		e => e.Key,
				//		ep => ep.EntryKey,
				//		(e, ep) => new { entry = e, entryPeriod = ep })
				//	.Join(unitOfWork.GetRepository<Period>().GetAll(),
				//		ep => ep.entryPeriod.PeriodKey,
				//		p => p.Key,
				//		(ep, p) => new { period = p, EntryPeriod = ep.entryPeriod })
				//	.Where(e => e.period.Year == year)
				//	.Select(e => e.EntryPeriod.Entry)
				//	.Distinct()
				//	.ToList();

				//var rootEntries = allActiveInYear.Where(e => e.ParentEntryKey == null);

				var allActiveInYear = unitOfWork.GetRepository<Entry>()
					.GetQuery()
					.Where(e => (e.ActiveFrom.Year <= year) &&
								(e.ActiveUntil == null || e.ActiveUntil.Value.Year >= year))
					.ToList();

				var rootEntries = allActiveInYear.Where(e => e.ParentEntryKey == null).ToList();
				foreach (var rootEntry in rootEntries)
				{
					foreach (var secondLevelEntry in allActiveInYear.Where(e => e.ParentEntryKey == rootEntry.Key))
					{
						rootEntry.ChildEntries.Add(secondLevelEntry);

						foreach (var thirdLevEntry in allActiveInYear.Where(e => e.ParentEntryKey == secondLevelEntry.Key))
						{
							secondLevelEntry.ChildEntries.Add(thirdLevEntry);
						}
					}
				}

				return rootEntries;
			}
		}

		public IList<Entry> GetRootEntries()
		{
			using (mUOWFactory.Create())
			{
				var root = GetAll()
					.Where(e => e.ParentEntry == null);

				return root.ToList();
			}
		}

		public IList<Entry> Get3rdLevelEntries()
		{
			using (mUOWFactory.Create())
			{
				var root = GetAll()
					.Join(GetAll(), e => e.ParentEntryKey, e2 => e2.Key, (e, e2) => new { Entry = e, Entry2 = e2 })
					.Join(GetAll(), e => e.Entry2.ParentEntryKey, e3 => e3.Key, (e, e3) => new { e.Entry })
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
			entity.ActiveFrom = new DateTime(DateTime.Now.Year, 1, 1);
			var result = base.Add(entity);

			// Then create a EntryPeriod for each period in the database.
			var periods = mPeriodDataService.Value.GetAll();

			foreach (var period in periods)
			{
				var newEntryPeriod = new EntryPeriod { Entry = entity, Period = period };
				mEntryPeriodDataService.Add(newEntryPeriod);
			}

			return result;
		}

		public override bool Delete(Entry entity)
		{
			var lastYearTheEntryWasUsed = GetLastYearTheEntryWasUsed(entity);
			// Never? Just delete the entry
			if (lastYearTheEntryWasUsed == null)
			{
				return base.Delete(entity);
			}

			// Otherwise mark it inactive
			entity.ActiveUntil = new DateTime(lastYearTheEntryWasUsed.Value + 1, 1, 1).AddTicks(-1);
			Update(entity);

			// Remove the entry from all years after the last year it was used.
			mEntryPeriodDataService.Delete(entity, lastYearTheEntryWasUsed.Value);

			// Remove linked processing rules
			mProcessingRuleDataService.DeleteByEntry(entity);

			return true;
		}

		private int? GetLastYearTheEntryWasUsed(Entry entity)
		{
			using (var uow = mUOWFactory.Create())
			{
				var result = uow.GetRepository<Entry>()
					.GetQuery(true)
					.Where(e => e.Key == entity.Key)
					.Join(uow.GetRepository<EntryPeriod>().GetAll(), e => e.Key, ep => ep.EntryKey,
						((e, ep) => new { entity = e, entityPeriod = ep }))
					.Join(uow.GetRepository<Period>().GetAll(), ep => ep.entityPeriod.PeriodKey, p => p.Key,
						(ep, p) => new { period = p, ep.entityPeriod })
					.Join(uow.GetRepository<Transaction>().GetAll(), pep => pep.entityPeriod.Key, t => t.EntryPeriodKey,
						(pep, t) => new { pep.period })
					.Max(p => (int?)p.period.Year);

				return result;
			}
		}
	}
}