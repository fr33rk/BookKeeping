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
        private readonly Lazy<IPeriodDataService> mPeriodDataService;
        private readonly IEntryPeriodDataService mEntryPeriodDataService;

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
                var root = GetAll().Where(e => e.ParentEntry == null);

                return root.ToList();
            }
        }

        private void loadChildren(IList<Entry> parents)
        {
            foreach (var parent in parents)
            {
                //parent.ChildEntries.
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
    }
}