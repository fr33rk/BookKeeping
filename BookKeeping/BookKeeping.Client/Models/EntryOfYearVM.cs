using PL.BookKeeping.Entities;
using PL.Common.Prism;

namespace BookKeeping.Client.Models
{
    public class EntryOfYearVM : ViewModelBase
    {
        public Entry entry { get; set; }

        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }


    }
}
