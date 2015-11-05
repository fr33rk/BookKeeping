using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure
{
    public class DataImportedEventArgs : EventArgs
    {
        public DataImportedEventArgs(int imported, int duplicate)
        {
            Imported = imported;
            Duplicate = duplicate;
        }

        public int Imported { get; private set; }
        public int Duplicate { get; private set; }

    }

}
