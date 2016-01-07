using System;

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