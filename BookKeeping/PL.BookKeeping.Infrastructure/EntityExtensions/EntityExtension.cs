using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.EntityExtensions
{
    public static class EntityExtension
    {
        public static int Level(this Entry entry)
        {
            if (entry.ParentEntry == null)
            {
                return 1;
            }
            else
            {
                return 1 + entry.ParentEntry.Level();
            }
        }

        public static Entry RootEntry(this Entry entry)
        {
            if (entry.ParentEntry == null)
            {
                return entry;
            }
            else
            {
                return entry.ParentEntry.RootEntry();
            }
        }

        public static bool UpdateChild(this Entry entry, Entry newChild)
        {
            var childEntry = entry.ChildEntries.FirstOrDefault(e => e.Key == newChild.Key);
            if (childEntry != null)
            {
                newChild.ParentEntry = childEntry.ParentEntry;
                childEntry = newChild;
                return true;
            }
            else
            {
                foreach (var child in entry.ChildEntries)
                {
                    if (child.UpdateChild(newChild))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
