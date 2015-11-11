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
    public class PeriodDataService : BaseTraceableObjectDataServiceOfT<Period>, IPeriodDataService
    {
        private static Dictionary<int, string> mMonthNames = new Dictionary<int, string>()
        {
            { 1, "Jan"},
            { 2, "Feb"},
            { 3, "Mrt"},
            { 4, "Apr"},
            { 5, "Mei"},
            { 6, "Jun"},
            { 7, "Jul"},
            { 8, "Aug"},
            { 9, "Sep"},
            { 10, "Okt"},
            { 11, "Nov"},
            { 12, "Dec"},
        };


        private IEntryDataService mEntryDataService;
        private IEntryPeriodDataService mEntryPeriodDataService;

        public PeriodDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService, IEntryDataService entryDataService,
            IEntryPeriodDataService entryPeriodDataService)
			: base(uowFactory, authorizationService)
		{
            mEntryDataService = entryDataService;
            mEntryPeriodDataService = entryPeriodDataService;
        }

        public Period AddUsingTransactionDate(DateTime transactionDate)
        {
            var retValue = new Period();

            retValue.PeriodStart = new DateTime(transactionDate.Year, transactionDate.Month, 2).AddTicks(-1);
            retValue.PeriodEnd = new DateTime(transactionDate.Year, transactionDate.Month, 1)
                .AddMonths(1).AddTicks(-1);
            retValue.Name = mMonthNames[retValue.PeriodStart.Month];

            base.Add(retValue);

            EntryPeriod newEntryPeriod;

            // Add a EntryPeriod object for all existing entries.
            foreach (var entry in mEntryDataService.GetAll())
            {
                newEntryPeriod = new EntryPeriod()
                {
                    Entry = entry,
                    Period = retValue,
                };

                mEntryPeriodDataService.Add(newEntryPeriod);
            }


            return retValue;
        }
    }
}
