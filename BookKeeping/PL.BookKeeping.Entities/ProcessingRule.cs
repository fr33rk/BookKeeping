using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    public class ProcessingRule : BaseTraceableObject
    {
        [Key, ForeignKey("Entry")]
        public int EntryKey { get; set; }

        public Entry Entry { get; set; }


        public bool AppliesTo(Transaction transaction)
        {
            throw new NotImplementedException();
        }


    }
}
