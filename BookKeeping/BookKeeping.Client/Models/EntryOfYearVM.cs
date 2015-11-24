using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.Common.Prism;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeping.Client.Models
{
    public class EntryOfYearVM : ViewModelBase
    {
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
                return mAmounts[(int)ShortMonthName.Jan];
            }
        }
        public decimal? Feb
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Feb];
            }
        }
        public decimal? Mar
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Mar];
            }
        }
        public decimal? Apr
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Apr];
            }
        }
        public decimal? May
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Mei];
            }
        }
        public decimal? Jun
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Jun];
            }
        }
        public decimal? Jul
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Jul];
            }
        }
        public decimal? Aug
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Aug];
            }
        }
        public decimal? Sep
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Sep];
            }
        }
        public decimal? Okt
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Okt];
            }
        }
        public decimal? Nov
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Nov];
            }
        }
        public decimal? Dec
        {
            get
            {
                return mAmounts[(int)ShortMonthName.Dec];
            }
        }


        #endregion Columns


        public decimal? Total
        {
            get
            {
                return mAmounts[mAmounts.Count() - 1];
            }
        }

        private decimal?[] mAmounts = new decimal?[13];
        private int?[] mPeriodKeys = new int?[13];

        public void SetPeriodData(IList<EntryPeriod> entryPeriods)
        {
            EntryPeriod entryPeriod;
            mAmounts[12] = 0;

            for (int i = 0; i < 12; i++)
            {
                entryPeriod = entryPeriods.FirstOrDefault(e => e.Period.PeriodStart.Month == (i + 1));

                if (entryPeriod != null)
                {
                    mAmounts[i] = entryPeriod.TotalAmount;
                    mAmounts[12] += entryPeriod.TotalAmount;
                }
                else
                {
                    mAmounts[i] = null;
                }
            }
        }

        public void AddToTotals(EntryOfYearVM entryOfYear)
        {
            for (int i = 0; i < mAmounts.Count(); i++)
            {
                if (mAmounts[i].HasValue)
                {
                    if (entryOfYear.mAmounts[i].HasValue)
                    {
                        mAmounts[i] += entryOfYear.mAmounts[i];
                    }
                    else
                    {
                        mAmounts[i] = entryOfYear.mAmounts[i];
                    }
                }
            }
        }
    }
}