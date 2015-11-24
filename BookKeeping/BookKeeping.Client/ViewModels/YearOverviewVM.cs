using System.Collections.Generic;
using BookKeeping.Client.Models;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using PL.BookKeeping.Entities;

namespace BookKeeping.Client.ViewModels
{
    public class YearOverviewVM : ViewModelBase
    {

        private IEntryPeriodDataService mEntryPeriodDataService;
        private IEntryDataService mEntryDataService;
        private ITransactionDataService mTransactionDataService;
        private int mYear;

        public YearOverviewVM(int year, IEntryPeriodDataService entryPeriodDataService, IEntryDataService entryDataService,
            ITransactionDataService transactionService)
        {
            mYear = year;
            mEntryPeriodDataService = entryPeriodDataService;
            mEntryDataService = entryDataService;
            mTransactionDataService = transactionService;

            loadData();
        }

        private EntryOfYearVM mSelectedEntryOfYear;

        public EntryOfYearVM SelectedEntryOfYear
        {
            get
            {
                return mSelectedEntryOfYear;
            }
            set
            {
                mSelectedEntryOfYear = value;
                NotifyPropertyChanged();
            }

        }

        private int mSelectedIndex;

        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                mSelectedIndex = value;
            }
        }

        private void LoadSelectedTransactions()
        {
            if ((mSelectedEntryOfYear != null)
                && ((SelectedIndex >= 1) || (SelectedIndex <= 12)))
            {
                mTransactionDataService.GetByEntryPeriod(mSelectedEntryOfYear.)
            }
        }



        private IEnumerable<Transaction> mSelectedTransactions;

        public IEnumerable<Transaction> SelectedTransactions
        {
            get
            {
                return mSelectedTransactions;
            }
            set
            {
                mSelectedTransactions = value;
                NotifyPropertyChanged();
            }
        }

        private void loadData()
        {
            mEntriesOfYear = new List<EntryOfYearVM>();

            // First flatten the list of entries
            var rootList = mEntryDataService.GetAllSorted();
            foreach (var rootEntry in rootList)
            {
                flatten(rootEntry);
            }

            // Then fill the entry periods.
            foreach(var entryOfYear in mEntriesOfYear)
            {
                var entryPeriods = mEntryPeriodDataService.GetByEntryAndYear(entryOfYear.Entry, mYear);

                entryOfYear.SetPeriodData(entryPeriods);
            }

            // Add the totals..
            var totalOut = new EntryOfYearVM(new Entry() { Description = "Total" });

            foreach (var entryOfYear in mEntriesOfYear)
            {
                totalOut.AddToTotals(entryOfYear);
            }

            mEntriesOfYear.Add(totalOut);

        }

        private void flatten(Entry entry)
        {
            mEntriesOfYear.Add(new EntryOfYearVM(entry));

            if (entry.ChildEntries != null)
            {
                foreach (var childEntry in entry.ChildEntries)
                {
                    flatten(childEntry);
                }
            }
        }

        private IList<EntryOfYearVM> mEntriesOfYear;

        public IList<EntryOfYearVM>EntriesOfYear
        {
            get
            {
                return mEntriesOfYear;
            }
        }

        public int Year
        {
            get
            {
                return mYear;
            }
        }
    }
}
