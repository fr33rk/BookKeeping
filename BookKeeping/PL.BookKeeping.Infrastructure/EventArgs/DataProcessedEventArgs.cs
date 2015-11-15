using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure
{
    public class DataProcessedEventArgs : EventArgs
    {
        public DataProcessedEventArgs(int processed)
        {
            Processed = processed;
        }

        public int Processed { get; private set; }
    }
}
