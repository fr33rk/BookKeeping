using PL.BookKeeping.Entities;
using PL.BookKeeping.Entities.Misc;
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
                retValue += string.Format("Naam = {0}", NameRule);
            }

            if (AccountRule != null)
            {
                retValue += string.Format("Rekening = {0}", AccountRule);
            }

            if (CounterAccountRule != null)
            {
                retValue += string.Format("Tegenrekening = {0}", CounterAccountRule);
            }

            if (CodeRule != null)
            {
                retValue += string.Format("Code = {0}", CodeRule);
            }

            if (MutationTypeRule != null)
            {
                retValue += string.Format("Af/bij = {0}", MutationTypeRule.ToString());
            }

            if (AmountRule != null)
            {
                retValue += string.Format("Bedrag = {0}", AmountRule.ToString());
            }

            if (RemarksRule != null)
            {
                retValue += string.Format("Opmerking = {0}", RemarksRule);
            }

            if (retValue == string.Empty)
            {
                retValue = "< Lege definitie >";
            }

            return retValue;
        }

        internal IList<Transaction> FilterList(ref IList<Transaction> transactions, int? year)
        {
            var rule = ToEntity();

            var retValue = transactions.Where(t =>
            {
                bool isMatch = true;

                if (year.HasValue)
                {
                    isMatch = t.Date.Year == year;
                }

                isMatch &= !string.IsNullOrEmpty(NameRule) ? Regex.IsMatch(t.Name, NameRule) : true;
                isMatch &= !string.IsNullOrEmpty(AccountRule) ? Regex.IsMatch(t.Account, AccountRule) : true;
                isMatch &= !string.IsNullOrEmpty(CounterAccountRule) ? Regex.IsMatch(t.CounterAccount, CounterAccountRule) : true;
                isMatch &= !string.IsNullOrEmpty(CodeRule) ? Regex.IsMatch(t.Code, CodeRule) : true;
                isMatch &= MutationTypeRule != null ? t.MutationType == MutationTypeRule : true;
                isMatch &= !string.IsNullOrEmpty(MutationKindRule) ? Regex.IsMatch(t.MutationKind, MutationKindRule) : true;
                isMatch &= !string.IsNullOrEmpty(RemarksRule) ? Regex.IsMatch(t.Remarks, RemarksRule) : true;
                isMatch &= ((AmountRule != null) && (!string.IsNullOrEmpty(AmountRule.ToString()))) ? rule.AmountRuleAppliesTo(t.Amount) : true;

                return isMatch;
            })
            .ToList();

            foreach (var transaction in retValue)
            {
                transactions.Remove(transaction);
            }

            return retValue;
        }

        #endregion ToString

        #region Clone

        public ProcessingRuleVM Clone()
        {
            return (ProcessingRuleVM)this.MemberwiseClone();
        }

        #endregion Clone        
    }
}