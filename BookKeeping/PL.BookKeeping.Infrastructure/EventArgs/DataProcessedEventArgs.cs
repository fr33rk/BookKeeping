using System;

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