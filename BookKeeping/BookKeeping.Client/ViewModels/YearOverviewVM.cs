using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Client.ViewModels
{
    public class YearOverviewVm : ViewModelBase
    {
        private readonly IEntryPeriodDataService mEntryPeriodDataService;
        private readonly IEntryDataService mEntryDataService;
        private readonly ITransactionDataService mTransactionDataService;

	    public YearOverviewVm(int year, IEntryPeriodDataService entryPeriodDataService, IEntryDataService entryDataService,
            ITransactionDataService transactionService)
        {
            Year = year;
            mEntryPeriodDataService = entryPeriodDataService;
            mEntryDataService = entryDataService;
            mTransactionDataService = transactionService;

            LoadData();
        }

        private EntryOfYearVm mSelectedEntryOfYear;

        public EntryOfYearVm SelectedEntryOfYear
        {
            get => mSelectedEntryOfYear;
	        set
            {
                mSelectedEntryOfYear = value;
                LoadSelectedTransactions();
            }
        }

        private uint mSelectedColumn;

        public uint SelectedColumn
        {
            get => mSelectedColumn;
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
            get => mSelectedTransactions;
	        set
            {
                mSelectedTransactions = value;
                NotifyPropertyChanged();
            }
        }

        private void LoadData()
        {
            EntriesOfYear = new List<EntryOfYearVm>();

            // First flatten the list of entries
            var rootList = mEntryDataService.GetRootEntries();
            foreach (var rootEntry in rootList)
            {
                Flatten(rootEntry);
            }

            // Then get all entries and periods of the selected year.
            var entryPeriods = mEntryPeriodDataService.GetByYear(Year);

            // Then fill the entry periods.
            foreach (var entryOfYear in EntriesOfYear)
            {
                var entryPeriodsOfentry = entryPeriods.Where(ep => ep.EntryKey == entryOfYear.Entry.Key).ToList();

                entryOfYear.SetPeriodData(entryPeriodsOfentry);
            }

            // Add the totals..
            var totalOut = new EntryOfYearVm(new Entry() { Description = "Total" });

            foreach (var rootEntry in rootList)
            {
                var entryOfYear = EntriesOfYear.First(eoy => eoy.Entry.Key == rootEntry.Key);
                totalOut.AddToTotals(entryOfYear);
            }

            EntriesOfYear.Add(totalOut);
        }

        private void Flatten(Entry entry)
        {
            EntriesOfYear.Add(new EntryOfYearVm(entry));

            if (entry.ChildEntries != null)
            {
                foreach (var childEntry in entry.ChildEntries.OrderBy(e => e.Description))
                {
                    Flatten(childEntry);
                }
            }
        }

	    public IList<EntryOfYearVm> EntriesOfYear { get; private set; }

	    public int Year { get; }
    }
}