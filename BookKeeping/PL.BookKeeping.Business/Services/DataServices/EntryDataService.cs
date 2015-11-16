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
        #region Constructor(s)

        public EntryDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
            : base(uowFactory, authorizationService)
        {
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

    }
}