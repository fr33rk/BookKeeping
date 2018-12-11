using Prism.Events;

namespace PL.BookKeeping.Infrastructure.EventAggregatorEvents
{
	public class ModuleInitializationDoneEvent : PubSubEvent<bool>
	{
	}
}