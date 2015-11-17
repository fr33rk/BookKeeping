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
            mEntryPeriods = mEntryPeriodDataService.GetAll(true);
        }

        public string Year
        {
            get { return mYear.ToString(); }
        }

        private IEnumerable<EntryPeriod> mEntryPeriods;
        

        public IEnumerable<EntryPeriod> EntryPeriods
        {
            get
            {
                return mEntryPeriods;
            }
        }
    }
}
