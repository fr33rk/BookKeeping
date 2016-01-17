using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static bool Replace<T>(this ObservableCollection<T> observableCollection, T replace, T withThis)
        {
            int index = observableCollection.IndexOf(replace);

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
                int swapThisIndex = observableCollection.IndexOf(swapThis);
                int withThatIndex = observableCollection.IndexOf(withThat);
                if ((swapThisIndex > -1) && (withThatIndex > -1))
                {
                    observableCollection[withThatIndex] = swapThis;
                    observableCollection[swapThisIndex] = withThat;
                    return;
                }
            }

            throw new InvalidOperationException(string.Format("Unable to swap {0} and {1}", swapThis.ToString(), withThat.ToString()));
        }
    }
}
