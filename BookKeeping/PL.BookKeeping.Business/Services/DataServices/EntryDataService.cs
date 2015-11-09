using System;
using System.Collections.Generic;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

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
    }
}