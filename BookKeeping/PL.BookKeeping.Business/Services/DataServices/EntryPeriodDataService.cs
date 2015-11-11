using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
