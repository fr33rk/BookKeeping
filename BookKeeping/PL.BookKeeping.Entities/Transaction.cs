using System;
using System.ComponentModel.DataAnnotations;

namespace PL.BookKeeping.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string CounterAccount { get; set; }
        public string Code { get; set; }
        public MutationType MutationType { get; set; }
        public decimal Amount { get; set; }
        public string MutationKind { get; set; }
        public string Remarks { get; set; }


    }
}