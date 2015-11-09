using System;
using System.ComponentModel.DataAnnotations;

namespace PL.BookKeeping.Entities
{
    public class Period 
    {
        [Key]
        public int Key { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, PeriodStart.Year);
        }
    }
}
