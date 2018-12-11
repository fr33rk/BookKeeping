using System;
using System.ComponentModel.DataAnnotations;

namespace PL.BookKeeping.Entities
{
	public class Period : BaseTraceableObject
	{
		#region Property Name

		[Required, StringLength(10)]
		public string Name { get; set; }

		#endregion Property Name

		#region Property PeriodStart

		[Required]
		public DateTime PeriodStart { get; set; }

		#endregion Property PeriodStart

		#region Property PeriodEnd

		[Required]
		public DateTime PeriodEnd { get; set; }

		#endregion Property PeriodEnd

		#region Property Year

		[Required]
		public int Year { get; set; }

		#endregion Property Year

		#region Helper methods

		public override string ToString()
		{
			return $"{Name} {PeriodStart.Year}";
		}

		#endregion Helper methods
	}
}