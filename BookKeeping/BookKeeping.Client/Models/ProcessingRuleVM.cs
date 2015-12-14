using PL.BookKeeping.Entities;
using System;

namespace BookKeeping.Client.Models
{
    public class ProcessingRuleVM
    {
        public int? EntryKey { get; set; }

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
    }
}