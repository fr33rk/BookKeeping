using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    [Table("Entry")]
    public class Entry : BaseTraceableObject
    {
        [StringLength(40)]
        public string Description { get; set; }

        [ForeignKey("ParentEntry")]
        public int? ParentEntryKey { get; set; }

        public Entry ParentEntry { get; set; }

        [InverseProperty("ParentEntry")]
        public ICollection<Entry> ChildEntries { get; set; }

		public DateTime ActiveFrom { get; set; }

	    public DateTime? ActiveUntil { get; set; }
    }
}