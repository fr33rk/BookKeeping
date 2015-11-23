using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.EventAggregatorEvents
{
    public class DataChangedEventArgs
    {
        public DataChangedEventArgs(bool allChanged, IList<int> changedYears)
        {
            AllChanged = allChanged;
            ChangedYears = changedYears;
        }

        public IList<int> ChangedYears { get; private set; }
        public bool AllChanged { get; private set; }
    }

    public class DataChangedEvent : PubSubEvent<DataChangedEventArgs>
    {
    }
}
