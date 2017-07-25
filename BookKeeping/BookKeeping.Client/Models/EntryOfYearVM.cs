using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.Common.Prism;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Client.Models
{
    public class EntryOfYearVm : ViewModelBase
    {
        private class Column
        {
            public Column(EntryPeriod entryPeriod, decimal amount)
            {
                EntryPeriod = entryPeriod;
                Amount = amount;
            }

            public readonly EntryPeriod EntryPeriod;
            public decimal Amount;
        }

        public EntryOfYearVm(Entry entry)
        {
            Entry = entry;
        }

        public Entry Entry { get; set; }

        #region Columns

        public decimal? Jan => GetAmountByIndex((int)ShortMonthName.Jan);

	    public decimal? Feb => GetAmountByIndex((int)ShortMonthName.Feb);

	    public decimal? Mar => GetAmountByIndex((int)ShortMonthName.Mar);

	    public decimal? Apr => GetAmountByIndex((int)ShortMonthName.Apr);

	    public decimal? May => GetAmountByIndex((int)ShortMonthName.Mei);

	    public decimal? Jun => GetAmountByIndex((int)ShortMonthName.Jun);

	    public decimal? Jul => GetAmountByIndex((int)ShortMonthName.Jul);

	    public decimal? Aug => GetAmountByIndex((int)ShortMonthName.Aug);

	    public decimal? Sep => GetAmountByIndex((int)ShortMonthName.Sep);

	    public decimal? Okt => GetAmountByIndex((int)ShortMonthName.Okt);

	    public decimal? Nov => GetAmountByIndex((int)ShortMonthName.Nov);

	    public decimal? Dec => GetAmountByIndex((int)ShortMonthName.Dec);

	    private decimal? GetAmountByIndex(int index)
        {
            if (mColumns[index] != null)
            {
                return mColumns[index].Amount;
            }
            return null;
        }

        #endregion Columns

        public decimal Total => mColumns[mColumns.Length - 1] != null 
									? mColumns[mColumns.Length - 1].Amount 
									: 0;

	    private readonly Column[] mColumns = new Column[13];

        public void SetPeriodData(IList<EntryPeriod> entryPeriods)
        {
	        mColumns[12] = new Column(null, 0);

            for (var i = 0; i < 12; i++)
            {
	            var entryPeriod = entryPeriods.FirstOrDefault(e => e.Period.PeriodStart.Month == (i + 1));

	            if (entryPeriod != null)
                {
                    mColumns[i] = new Column(entryPeriod, entryPeriod.TotalAmount);
                    mColumns[12].Amount += entryPeriod.TotalAmount;
                }
                else
                {
                    mColumns[i] = null;
                }
            }
        }

        public void AddToTotals(EntryOfYearVm entryOfYear)
        {
            for (var i = 0; i < mColumns.Length; i++)
            {
                if (mColumns[i] == null)
                {
                    mColumns[i] = new Column(null, 0);
                }

                if (entryOfYear.mColumns[i] != null)
                {
                    mColumns[i].Amount += entryOfYear.mColumns[i].Amount;
                }
                else
                {
                    if (entryOfYear.mColumns[i] != null)
                    {
                        mColumns[i].Amount = entryOfYear.mColumns[i].Amount;
                    }
                }
            }
        }

        public int Level
        {
            get
            {
                if (Entry == null)
                {
                    return 0;
                }

                if (Entry.ParentEntry == null)
                {
                    return 1;
                }
                else if ((Entry.ParentEntry != null) && (Entry.ChildEntries != null))
                {
                    return 2;
                }
                else if ((Entry.ParentEntry != null) && (Entry.ChildEntries == null))
                {
                    return 3;
                }

                // Should not get here...
                return -1;
            }
        }

        public EntryPeriod GetEntryPeriodByColumn(uint index)
        {
            if (index < mColumns.Length)
            {
                if (mColumns[index] != null)
                {
                    return mColumns[index].EntryPeriod;
                }
            }

            return null;
        }
    }
}