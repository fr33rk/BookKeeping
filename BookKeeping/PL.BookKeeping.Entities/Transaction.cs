using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PL.BookKeeping.Entities.Traceability;

namespace PL.BookKeeping.Entities
{
	public class Transaction : BaseTraceableObject
	{
		[Required]
		public DateTime Date { get; set; }

		[Required, StringLength(100)]
		public string Name { get; set; }

		[Required, StringLength(18)]
		public string Account { get; set; }

		[StringLength(27)]
		public string CounterAccount { get; set; }

		[StringLength(2)]
		public string Code { get; set; }

		[Required]
		public MutationType MutationType { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[StringLength(20)]
		public string MutationKind { get; set; }

		[StringLength(500)]
		public string Remarks { get; set; }

		[ForeignKey("EntryPeriod")]
		public int? EntryPeriodKey { get; set; }

		public EntryPeriod EntryPeriod { get; set; }

		#region Property Fingerprint

		private int mFingerPrint;

		public int FingerPrint
		{
			get
			{
				mFingerPrint =
					(Date.Year % 100) * 100000000 +
					Date.Month * 1000000 +
					Date.Day * 10000 +
					(Decimal.ToInt32(Amount * 100)) % 10000;

				return mFingerPrint;
			}
			set => mFingerPrint = value;
		}

		#endregion Property Fingerprint

		public bool IsEqual(Transaction theOther)
		{
			return (Date.Equals(theOther.Date) &&
				Name.Equals(theOther.Name) &&
				Account.Equals(theOther.Account) &&
				Amount.Equals(theOther.Amount) &&
				Remarks.Equals(theOther.Remarks));
		}

		public override string ToString()
		{
			return $"{Date.ToString()}, {Name}, {Amount}";
		}
	}
}