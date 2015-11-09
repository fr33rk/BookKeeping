using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace PL.BookKeeping.Entities
{
    public class ProcessingRule : BaseTraceableObject
    {
        [Key, ForeignKey("Entry")]
        public int EntryKey { get; set; }

        public Entry Entry { get; set; }

        public string NameRule { get; set; }

        public string AccountRule { get; set; }

        public string CounterAccountRule { get; set; }

        public string CodeRule { get; set; }

        public string AmountRule { get; set; }

        public string MutationKindRule { get; set; }

        public string RemarksRule { get; set; }

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

            return retValue;

        }


    }
}
