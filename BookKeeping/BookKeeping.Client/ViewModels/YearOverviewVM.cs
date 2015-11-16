using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookKeeping.Client.ViewModels
{
    public class YearOverviewVM : ViewModelBase
    {

        private IEntryPeriodDataService mEntryPeriodDataService;
        private IEntryDataService mEntryDataService;
        private int mYear;
        

        public YearOverviewVM(int year, IEntryPeriodDataService entryPeriodDataService, IEntryDataService entryDataService)
        {
            mYear = year;
            mEntryPeriodDataService = entryPeriodDataService;
            mEntryDataService = entryDataService;

            loadData();
        }

        private void loadData()
        {
            mEntries = mEntryDataService.GetAllSorted();
        }

        public string Year
        {
            get { return mYear.ToString(); }
        }

        private IEnumerable<Entry> mEntries;
        

        public IEnumerable<Entry> Entries
        {
            get
            {
                return mEntries;
            }
        }
    }
}
