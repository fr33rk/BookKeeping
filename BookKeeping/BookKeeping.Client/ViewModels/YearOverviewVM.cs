using System.Collections.Generic;
using BookKeeping.Client.Models;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;

namespace BookKeeping.Client.ViewModels
{
    public class YearOverviewVM : ViewModelBase
    {

        private IEntryPeriodDataService mEntryPeriodDataService;
        private IEntryDataService mEntryDataService;
        private int mYear;

        private IList<EntryOfYearVM> mEntriesOfYear;
        

        public YearOverviewVM(int year, IEntryPeriodDataService entryPeriodDataService, IEntryDataService entryDataService)
        {
            mYear = year;
            mEntryPeriodDataService = entryPeriodDataService;
            mEntryDataService = entryDataService;

            loadData();
        }

        private void loadData()
        {
            mEntryPeriodDataService.GetAll(true);
        }


    }
}
