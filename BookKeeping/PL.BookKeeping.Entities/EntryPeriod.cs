using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    public class EntryPeriod : BaseTraceableObject
    {
        [Key, Column(Order=0), ForeignKey("Entry")]
        public int EntryKey { get; set; }

        [Key, Column(Order = 1), ForeignKey("Period")]
        public int PeriodKey { get; set; }

        [Required]
        public Entry Entry { get; set; }

        [Required]
        public Period Period { get; set; }

        [InverseProperty("EntryPeriod")]
        public ICollection<Transaction> Transactions { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
