using System;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Entities.Misc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookKeeping.Client.Models
{
	/// <inheritdoc />
	///  <summary>Wrapper class around  the ProcessingRule entity.
	///  </summary>
	public class ProcessingRuleVm : BaseTraceableObjectVMOfT<ProcessingRule, ProcessingRuleVm>
	{
		#region Fields

		private readonly AmountRule mAmountRule;

		#endregion Fields

		#region Constructor(s)

		public ProcessingRuleVm()
		{
			mAmountRule = new AmountRule();
		}

		#endregion Constructor(s)

		#region ProcessingRule entity

		public int? EntryKey { get; set; }

		public Entry Entry { get; set; }

		public int Priority { get; set; }

		public DateTime? DateBeforeRule { get; set; }

		public DateTime? DateAfterRule { get; set; }

		public string NameRule { get; set; }

		public string AccountRule { get; set; }

		public string CounterAccountRule { get; set; }

		public string CodeRule { get; set; }

		public MutationType? MutationTypeRule { get; set; }

		public string AmountRule
		{
			get => mAmountRule.ToString();
			set
			{
				// TODO: Handle invalid amount rule strings properly. In case of an empty string the AmountRule should be cleared.
				mAmountRule.FromString(value);
				NotifyPropertyChanged();
			}
		}

		public string MutationKindRule { get; set; }

		public string RemarksRule { get; set; }

		#endregion ProcessingRule entity

		#region ToString

		/// <summary>Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			var rules = new List<string>();

			if (NameRule != null)
			{
				rules.Add($"Naam = {NameRule}");
			}

			if (AccountRule != null)
			{
				rules.Add($"Rekening = {AccountRule}");
			}

			if (CounterAccountRule != null)
			{
				rules.Add($"Tegenrekening = {CounterAccountRule}");
			}

			if (CodeRule != null)
			{
				rules.Add($"Code = {CodeRule}");
			}

			if (MutationTypeRule != null)
			{
				rules.Add($"Af/bij = {MutationTypeRule.ToString()}");
			}

			if (AmountRule != null)
			{
				rules.Add($"Bedrag = {AmountRule}");
			}

			if (RemarksRule != null)
			{
				rules.Add($"Opmerking = {RemarksRule}");
			}

			return rules.Count == 0
				? "< Lege definitie >"
				: string.Join("; ", rules);
		}

		//internal IList<Transaction> FilterList(ref IList<Transaction> transactions, int? year)
		//{
		//	var rule = ToEntity();

		//	var retValue = transactions.Where(t =>
		//	{
		//		var isMatch = true;

		//		if (year.HasValue)
		//		{
		//			isMatch = t.Date.Year == year;
		//		}

		//		isMatch &= !string.IsNullOrEmpty(NameRule) ? Regex.IsMatch(t.Name, NameRule) : true;

		//		if (isMatch)
		//		{
		//			isMatch &= !string.IsNullOrEmpty(AccountRule) ? Regex.IsMatch(t.Account, AccountRule) : true;
		//			isMatch &= !string.IsNullOrEmpty(CounterAccountRule) ? Regex.IsMatch(t.CounterAccount, CounterAccountRule) : true;
		//			isMatch &= !string.IsNullOrEmpty(CodeRule) ? Regex.IsMatch(t.Code, CodeRule) : true;
		//			isMatch &= MutationTypeRule != null ? t.MutationType == MutationTypeRule : true;
		//			isMatch &= !string.IsNullOrEmpty(MutationKindRule) ? Regex.IsMatch(t.MutationKind, MutationKindRule) : true;
		//			isMatch &= !string.IsNullOrEmpty(RemarksRule) ? Regex.IsMatch(t.Remarks, RemarksRule) : true;
		//			isMatch &= ((AmountRule != null) && (!string.IsNullOrEmpty(AmountRule.ToString()))) ? rule.AmountRuleAppliesTo(t.Amount) : true;
		//		}
		//		return isMatch;
		//	})
		//	.ToList();

		//	foreach (var transaction in retValue)
		//	{
		//		transactions.Remove(transaction);
		//	}

		//	return retValue;
		//}

		#endregion ToString

		#region Clone

		public ProcessingRuleVm Clone()
		{
			return (ProcessingRuleVm)MemberwiseClone();
		}

		#endregion Clone

		internal ProcessingRuleVm NullifyEmptyStrings()
		{
			if (string.IsNullOrEmpty(NameRule))
			{
				NameRule = null;
			}

			if (string.IsNullOrEmpty(AccountRule))
			{
				AccountRule = null;
			}

			if (string.IsNullOrEmpty(CounterAccountRule))
			{
				CounterAccountRule = null;
			}

			if (string.IsNullOrEmpty(CodeRule))
			{
				CodeRule = null;
			}

			if (string.IsNullOrEmpty(MutationKindRule))
			{
				MutationKindRule = null;
			}

			if (string.IsNullOrEmpty(RemarksRule))
			{
				RemarksRule = null;
			}

			return this;
		}
	}
}