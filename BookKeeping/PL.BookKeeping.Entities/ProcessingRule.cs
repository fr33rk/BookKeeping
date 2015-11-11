using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace PL.BookKeeping.Entities
{
    public class ProcessingRule : BaseTraceableObject
    {
        [ForeignKey("Entry")]
        public int EntryKey { get; set; }

        public Entry Entry { get; set; }

        public int Priority { get; set; }

        public string NameRule { get; set; }

        public string AccountRule { get; set; }

        public string CounterAccountRule { get; set; }

        public string CodeRule { get; set; }

        public MutationType? MutationTypeRule { get; set; }

        public string AmountRule { get; set; }

        public string MutationKindRule { get; set; }

        public string RemarksRule { get; set; }
        
        public int Priotiry { get; set; }

        /// <summary>Check if this rule applies to the given transaction.</summary>
        /// <param name="transaction">The transaction that needs to be checked.</param>
        /// <returns>True, when the rule applies.</returns>
        public bool AppliesTo(Transaction transaction)
        {
            bool retValue = true;

            if (NameRule != null)
                retValue &= Regex.IsMatch(transaction.Name, NameRule);

            if (retValue && AccountRule != null)
                retValue &= Regex.IsMatch(transaction.Account, AccountRule);

            if (retValue && CounterAccountRule != null)
                retValue &= Regex.IsMatch(transaction.CounterAccount, CounterAccountRule);

            if (retValue && CodeRule != null)
                retValue &= Regex.IsMatch(transaction.Code, CodeRule);

            if (retValue && MutationTypeRule.HasValue)
                retValue &= transaction.MutationType == MutationTypeRule;

            if (retValue && AmountRule != null)
                retValue &= AmountRuleAppliesTo(transaction.Amount);

            if (retValue && MutationKindRule != null)
                retValue &= Regex.IsMatch(transaction.MutationKind, MutationKindRule);

            if (retValue && RemarksRule != null)
                retValue &= Regex.IsMatch(transaction.Remarks, RemarksRule);

            return retValue;

        }

        private bool AmountRuleAppliesTo(decimal amount)
        {
            // format like minValue [< | <= | >= | >] x
            // combined with x [< | <= | = | >= | >]
            throw new NotImplementedException();
        }

        /// <summary>Check if the amount rule has a valid format.
        /// </summary>
        /// <param name="amountRule">The amount rule to check.</param>
        /// <returns>True, when the amount rule is valid.</returns>
        public bool IsValidAmountRule(string amountRule)
        {
            throw new NotImplementedException();
        }
    }
}
