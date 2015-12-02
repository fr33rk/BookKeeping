using System;
using System.Collections.Generic;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class EntryDataService : BaseTraceableObjectDataServiceOfT<Entry>, IEntryDataService
    {
        private Lazy<IPeriodDataService> mPeriodDataService;
        private IEntryPeriodDataService mEntryPeriodDataService;


        #region Constructor(s)

        public EntryDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService,
            Lazy<IPeriodDataService> periodDataService, IEntryPeriodDataService entryPeriodDataService)
            : base(uowFactory, authorizationService)
        {
            mPeriodDataService = periodDataService;
            mEntryPeriodDataService = entryPeriodDataService;
        }

        #endregion Constructor(s)

        public IList<Entry> GetAllSorted()
        {
            using (var unitOfWork = this.mUOWFactory.Create())
            {
                var root = GetAll().Where(e => e.ParentEntry == null);
                //var repository = unitOfWork.GetRepository<Entry>();
                //repository.

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

        public override void Add(Entry entity)
        {
            // First add the entry to the database.
            base.Add(entity);

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

        }

    }
}