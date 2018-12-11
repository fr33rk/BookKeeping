using Prism.Events;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.EventAggregatorEvents
{
	public class DataChangedEventArgs
	{
		public DataChangedEventArgs()
		{
			ChangedYears = null;
		}

		public DataChangedEventArgs(IList<int> changedYears)
		{
			ChangedYears = changedYears;
		}

		public IList<int> ChangedYears { get; private set; }
	}

	public class DataChangedEvent : PubSubEvent<DataChangedEventArgs>
	{
	}
}