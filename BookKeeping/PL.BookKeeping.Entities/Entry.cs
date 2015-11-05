using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    public partial class Entry : BaseTraceableObject
    {
        [StringLength(40)]
        public string Description { get; set; }

        [InverseProperty("Entry")]
        public ICollection<Entry> ChildEntries { get; set; }


    }
}
