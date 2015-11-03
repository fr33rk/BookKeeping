using System;
using System.ComponentModel.DataAnnotations;

namespace PL.BookKeeping.Entities
{
    public class Transaction : BaseTraceableObject
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string CounterAccount { get; set; }
        public string Code { get; set; }
        public MutationType MutationType { get; set; }
        public decimal Amount { get; set; }
        public string MutationKind { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        #region Property Fingerprint

        private int mFingerPrint;

        public int FingerPrint
        {
            get
            {
                mFingerPrint =
                    (Date.Year % 100) * 100000000 +
                    Date.Month *1000000 +
                    Date.Day * 10000 +
                    (Decimal.ToInt32(Amount * 100)) % 10000;

                return mFingerPrint;
            }
            set
            {
                mFingerPrint = value;
            }
        }

        #endregion

        public bool IsEqual(Transaction theOther)
        {
            return (Date.Equals(theOther.Date) &&
                Name.Equals(theOther.Name) &&
                Account.Equals(theOther.Account) &&
                Amount.Equals(theOther.Amount));
        }

    }
}