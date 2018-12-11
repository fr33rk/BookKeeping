using Prism.Events;

namespace PL.BookKeeping.Infrastructure.EventAggregatorEvents
{
	public class OptionsChangedEvent : PubSubEvent<Settings>
	{
	}
}