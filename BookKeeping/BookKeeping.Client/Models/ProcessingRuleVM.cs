using PL.BookKeeping.Entities;
using PL.BookKeeping.Entities.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookKeeping.Client.Models
{
    /// <summary>Wrapper class around  the ProcessingRule entity.
    ///
    /// </summary>
    public class ProcessingRuleVM : BaseTracableObjectVMOfT<ProcessingRule, ProcessingRuleVM>
    {
        #region Fields

        private AmountRule mAmountRule;

        #endregion Fields

        #region Constructor(s)

        public ProcessingRuleVM()
            : base()
        {
            mAmountRule = new AmountRule();
        }

        #endregion Constructor(s)

        #region ProcessingRule entity

        public int? EntryKey { get; set; }

        public Entry Entry { get; set; }

        public int Priority { get; set; }

        public string NameRule { get; set; }

        public string AccountRule { get; set; }

        public string CounterAccountRule { get; set; }

        public string CodeRule { get; set; }

        public MutationType? MutationTypeRule { get; set; }

        public string AmountRule
        {
            get
            {
                return mAmountRule.ToString();
            }
            set
            {
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
            string retValue = "";

            if (NameRule != null)
            {
                retValue += String.Format("Naam = {0}", NameRule);
            }

            if (AccountRule != null)
            {
                retValue += String.Format("Rekening = {0}", AccountRule);
            }

            if (CounterAccountRule != null)
            {
                retValue += String.Format("Tegenrekening = {0}", CounterAccountRule);
            }

            if (CodeRule != null)
            {
                retValue += String.Format("Code = {0}", CodeRule);
            }

            if (MutationTypeRule != null)
            {
                retValue += String.Format("Af/bij = {0}", MutationTypeRule.ToString());
            }

            if (AmountRule != null)
            {
                retValue += String.Format("Bedrag = {0}", AmountRule);
            }

            if (RemarksRule != null)
            {
                retValue += String.Format("Opmerking = {0}", AmountRule);
            }

            return retValue;
        }

        internal IList<Transaction> FilterList(ref IList<Transaction> transactions)
        {
            var rule = ToEntity();

            var retValue = transactions.Where(t =>
            {
                return NameRule != null ? Regex.IsMatch(t.Name, NameRule) : true
                && AccountRule != null ? Regex.IsMatch(t.Account, AccountRule) : true
                && CounterAccountRule != null ? Regex.IsMatch(t.CounterAccount, CounterAccountRule) : true
                && CodeRule != null ? Regex.IsMatch(t.Code, CodeRule) : true
                && MutationTypeRule != null ? t.MutationType == MutationTypeRule : true
                && MutationKindRule != null ? Regex.IsMatch(t.MutationKind, MutationKindRule) : true
                && RemarksRule != null ? Regex.IsMatch(t.Remarks, RemarksRule) : true
                && rule.AmountRuleAppliesTo(t.Amount);
            })
            .ToList();

            foreach (var transaction in retValue)
            {
                transactions.Remove(transaction);
            }

            return retValue;
        }

        #endregion ToString
    }
}