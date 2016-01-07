using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.Common.Prism;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Client.Models
{
    public class EntryOfYearVM : ViewModelBase
    {
        private class column
        {
            public column(EntryPeriod entryPeriod, decimal amount)
            {
                EntryPeriod = entryPeriod;
                Amount = amount;
            }

            public EntryPeriod EntryPeriod;
            public decimal Amount;
        }

        public EntryOfYearVM(Entry entry)
        {
            Entry = entry;
        }

        public Entry Entry { get; set; }

        #region Columns

        public decimal? Jan
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Jan);
            }
        }

        public decimal? Feb
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Feb);
            }
        }

        public decimal? Mar
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Mar);
            }
        }

        public decimal? Apr
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Apr);
            }
        }

        public decimal? May
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Mei);
            }
        }

        public decimal? Jun
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Jun);
            }
        }

        public decimal? Jul
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Jul);
            }
        }

        public decimal? Aug
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Aug);
            }
        }

        public decimal? Sep
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Sep);
            }
        }

        public decimal? Okt
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Okt);
            }
        }

        public decimal? Nov
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Nov);
            }
        }

        public decimal? Dec
        {
            get
            {
                return GetAmountByIndex((int)ShortMonthName.Dec);
            }
        }

        private decimal? GetAmountByIndex(int index)
        {
            if (mColumns[index] != null)
            {
                return mColumns[index].Amount;
            }
            return null;
        }

        #endregion Columns

        public decimal Total
        {
            get
            {
                if (mColumns[mColumns.Count() - 1] != null)
                {
                    return mColumns[mColumns.Count() - 1].Amount;
                }

                return 0;
            }
        }

        private column[] mColumns = new column[13];

        public void SetPeriodData(IList<EntryPeriod> entryPeriods)
        {
            EntryPeriod entryPeriod;
            mColumns[12] = new column(null, 0);

            for (int i = 0; i < 12; i++)
            {
                entryPeriod = entryPeriods.FirstOrDefault(e => e.Period.PeriodStart.Month == (i + 1));

                if (entryPeriod != null)
                {
                    mColumns[i] = new column(entryPeriod, entryPeriod.TotalAmount);
                    mColumns[12].Amount += entryPeriod.TotalAmount;
                }
                else
                {
                    mColumns[i] = null;
                }
            }
        }

        public void AddToTotals(EntryOfYearVM entryOfYear)
        {
            for (int i = 0; i < mColumns.Count(); i++)
            {
                if (mColumns[i] == null)
                {
                    mColumns[i] = new column(null, 0);
                }
                else
                {
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
            if (index < mColumns.Count())
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