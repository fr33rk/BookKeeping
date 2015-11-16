using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace PL.BookKeeping.Infrastructure.EventAggregatorEvents
{
    public class ModuleInitializationDoneEvent : PubSubEvent<bool>
    {
    }
}
