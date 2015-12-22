using System;
using PL.BookKeeping.Entities;

namespace BookKeeping.Client.Models
{
	/// <summary>VM equivalent of a BaseTracableObject entry.
	/// </summary>
	public class BaseTracableObjectVM
	{
		/// <summary>The unique identifier for this object. Should never be manually set as it is
		/// manager by the Entity Framework.
		/// </summary>
		public int Key { get; set; }

		/// <summary>Gets or sets the creation date and time. Should never be manually set.
		/// </summary>
		public DateTime CreationDT { get; set; }

		/// <summary>Gets or sets the creator key.</summary>
		public int? CreatorKey { get; set; }

		/// <summary>Gets or sets the creator. Should never be manually set.
		/// </summary>
		public virtual User Creator { get; set; }
	}
}