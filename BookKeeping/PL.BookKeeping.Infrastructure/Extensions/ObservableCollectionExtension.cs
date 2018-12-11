using System;
using System.Collections.ObjectModel;

namespace PL.BookKeeping.Infrastructure.Extensions
{
	public static class ObservableCollectionExtension
	{
		public static bool Replace<T>(this ObservableCollection<T> observableCollection, T replace, T withThis)
		{
			var index = observableCollection.IndexOf(replace);

			if (index >= 0)
			{
				observableCollection.RemoveAt(index);
				observableCollection.Insert(index, withThis);
				return true;
			}

			return false;
		}

		public static void Swap<T>(this ObservableCollection<T> observableCollection, T swapThis, T withThat)
		{
			if ((swapThis != null) && (withThat != null))
			{
				var swapThisIndex = observableCollection.IndexOf(swapThis);
				var withThatIndex = observableCollection.IndexOf(withThat);
				if ((swapThisIndex > -1) && (withThatIndex > -1))
				{
					observableCollection[withThatIndex] = swapThis;
					observableCollection[swapThisIndex] = withThat;
					return;
				}
			}

			throw new InvalidOperationException($"Unable to swap {swapThis.ToString()} and {withThat.ToString()}");
		}
	}
}