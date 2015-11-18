using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure
{
    public class DataProcessedEventArgs : EventArgs
    {
        public DataProcessedEventArgs(int processed, int ignored)
        {
            Processed = processed;
            Ignored = ignored;
        }

        public int Processed { get; private set; }
        public int Ignored { get; private set; }
    }
}
