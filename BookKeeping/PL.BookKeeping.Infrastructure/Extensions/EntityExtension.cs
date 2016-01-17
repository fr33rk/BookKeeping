using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Infrastructure.Extensions
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
    }
}