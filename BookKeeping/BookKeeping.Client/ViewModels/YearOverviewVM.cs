using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using System.Collections.Generic;
using System.Linq;

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
                LoadSelectedTransactions();
            }
        }

        private uint mSelectedColumn;

        public uint SelectedColumn
        {
            get
            {
                return mSelectedColumn;
            }
            set
            {
                mSelectedColumn = value;
                LoadSelectedTransactions();
            }
        }

        private void LoadSelectedTransactions()
        {
            if (mSelectedEntryOfYear != null)
            {
                SelectedTransactions = mTransactionDataService.GetByEntryPeriod(mSelectedEntryOfYear.GetEntryPeriodByColumn(mSelectedColumn - 1));
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
            var rootList = mEntryDataService.GetRootEntries();
            foreach (var rootEntry in rootList)
            {
                flatten(rootEntry);
            }

            // Then get all entries and periods of the selected year.
            var entryPeriods = mEntryPeriodDataService.GetByYear(mYear);


            // Then fill the entry periods.
            foreach (var entryOfYear in mEntriesOfYear)
            {
                var entryPeriodsOfentry = entryPeriods.Where(ep => ep.EntryKey == entryOfYear.Entry.Key).ToList();

                entryOfYear.SetPeriodData(entryPeriodsOfentry);
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
                foreach (var childEntry in entry.ChildEntries.OrderBy(e => e.Description))
                {
                    flatten(childEntry);
                }
            }
        }

        private IList<EntryOfYearVM> mEntriesOfYear;

        public IList<EntryOfYearVM> EntriesOfYear
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